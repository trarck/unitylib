# -*- coding: utf-8 -*-

import argparse
import json
import os.path
import shutil
import sys
from datetime import datetime
import requests

py_proj_root = os.path.dirname(os.path.dirname(__file__))
if py_proj_root not in sys.path:
    sys.path.append(py_proj_root)

from Defines import ErrorCode, HttpDomains, BuildPaths
from Util import RepositoryUtil, mini_profile, CmdUtil, PathUtil, CommonUtil

from CIReport.ci_sql_connector import CISQLConnector


class RepoCommitInfo:
    """
    工程仓库提交信息
    """

    def __init__(self, name=None, revision=None, paths=None):
        self.name = name
        self.revision = revision
        # 工程路径
        self.paths = paths
        self.priority = -1

        self.repo_url = None
        self.latest_revision = -1
        self.success_revision = -1
        self.logs = None
        self.step_index = -1

    def to_dict(self):
        d = {'name': self.name, 'revision': self.revision, 'paths': self.paths, 'priority': self.priority}
        return d

    def from_dict(self, d):
        self.name = d['name']
        self.revision = d['revision']
        self.paths = d['paths']
        self.priority = d['priority']

    def is_end(self):
        if not self.logs:
            return True
        return self.step_index < len(self.logs) - 1

    def next_step(self):
        self.step_index += 1

    def get_current_log(self):
        if self.logs and self.step_index < len(self.logs):
            return self.logs[self.step_index]
        return None


class BuildStepInfo:
    """
    构建相关信息
    """

    def __init__(self, commit_info: RepoCommitInfo, repo_log):
        self.commit_info = commit_info
        self.repo_log = repo_log
        self.build_results: list[BuildResult] = []

        # 失败的构建结果
        self.fail_build_results: dict[str, BuildResult] = dict()

    def get_commit_revision(self):
        return self.repo_log['revision'] if self.repo_log else -1

    def get_unique_key(self):
        return f"{self.commit_info.name}_{self.repo_log['revision']}"

    def is_success(self):
        return len(self.fail_build_results) == 0

    def get_all_error_str(self):
        all_error_str = ""
        for build_config, build_result in self.fail_build_results.items():
            all_error_str += f"-------------{build_config}-------------\n"
            all_error_str += build_result.get_error() + "\n"
        return all_error_str

    def get_brief_error_str(self):
        brief_error_str = ""
        for build_config, build_result in self.fail_build_results.items():
            error_lines = build_result.get_error_lines()
            if len(error_lines) > 5:
                brief_error_str = "\n".join(error_lines[0:5])
            else:
                brief_error_str = "\n".join(error_lines)
        return brief_error_str

    def get_repo_name(self):
        return self.commit_info.name

class ProjectClient:
    commit_info_file = "config/commits.json"

    def __init__(self, workspace, project_prefix, branch_version):
        self.workspace = workspace
        self.project_prefix = project_prefix
        self.branch_version = branch_version

        self.branch_root_path = None
        self.project_root_path = None
        self.main_branch_url = None
        self.scripts_branch_url = None
        self.client_code_branch_url = None
        self.ml_plugin_branch_url = None

        self.commit_infos: dict[str, RepoCommitInfo] = {}
        self.priority_commit_infos: list[RepoCommitInfo] = []

        self.project_cmake_checker_root_path = None

    def setup(self, clean_cache=False):
        # Android-xx
        branch_name = RepositoryUtil.get_branch_root_name(self.branch_version)
        # G:/autobuild/Android-xxx
        self.branch_root_path = os.path.join(self.workspace, branch_name)
        # mobaclient-xxx
        branch_project_name = RepositoryUtil.get_branch_project_name(self.branch_version, self.project_prefix)
        # G:/autobuild/Android-xxx/mobaclient-xxx
        self.project_root_path = os.path.join(self.branch_root_path, branch_project_name)
        self.main_branch_url = RepositoryUtil.get_main_branch_url(self.branch_version)
        self.scripts_branch_url = RepositoryUtil.get_logic_branch_url(self.branch_version)
        self.client_code_branch_url = RepositoryUtil.get_client_code_scripts_branch_url(self.branch_version)
        self.ml_plugin_branch_url = RepositoryUtil.get_plugin_scripts_branch_url(self.branch_version)

        #
        self.project_cmake_checker_root_path = os.path.join(self.project_root_path, "Tools/ProjectCompileChecker")

    def clean_cache(self):
        cmake_bin_folder_path = os.path.join(self.project_cmake_checker_root_path, "bin")
        PathUtil.clean_tree(cmake_bin_folder_path)

        cmake_build_folder_path = os.path.join(self.project_cmake_checker_root_path, "build")
        PathUtil.clean_tree(cmake_build_folder_path)

        cmake_backup_folder_path = os.path.join(self.project_cmake_checker_root_path, "backup")
        PathUtil.clean_tree(cmake_backup_folder_path)

    def prepare_build_project(self, fore_update=True):
        # check the project exists
        if os.path.exists(self.project_root_path):
            print(f"the project {self.project_root_path} exists.")

            # check sub folders
            if os.path.exists(os.path.join(self.project_root_path, "Assets")):
                if fore_update:
                    RepositoryUtil.update_path_depth(self.project_cmake_checker_root_path)
                    RepositoryUtil.resolve_conflicts(self.project_cmake_checker_root_path)
                return

        print(f"check out to {self.project_root_path}")
        # 检查svn地址是否正确
        self._check_svn(self.main_branch_url)

        # check the branch root
        if not os.path.exists(self.branch_root_path):
            os.makedirs(self.branch_root_path)

        # checkout project
        self._checkout_part_project(self.project_root_path, self.main_branch_url, self.scripts_branch_url)

    @staticmethod
    @mini_profile()
    def _checkout_part_project(project_root_path, main_branch_url, scripts_branch_url):
        # main
        RepositoryUtil.checkout_depth(main_branch_url, project_root_path, "empty", quiet=True)
        RepositoryUtil.update_path_depth(os.path.join(project_root_path, "Tools/ProjectCompileChecker"), "infinity")
        RepositoryUtil.update_path_depth(os.path.join(project_root_path, "Assets"), "files")
        RepositoryUtil.update_path_depth(os.path.join(project_root_path, "Assets/Plugins"), "infinity")
        RepositoryUtil.update_path_depth(os.path.join(project_root_path, "Assets/Editor"), "infinity")
        RepositoryUtil.update_path_depth(os.path.join(project_root_path, "Assets/Tools"), "infinity")
        RepositoryUtil.update_path_depth(os.path.join(project_root_path, "Packages"), "infinity")
        # scripts
        RepositoryUtil.checkout_depth(scripts_branch_url, os.path.join(project_root_path, "Assets/Scripts"), "infinity",
                                      quiet=True)

    @staticmethod
    def _check_svn(branch_url):
        if not RepositoryUtil.is_url_exits(branch_url):
            print(f"Error: svn {branch_url} does not, abort")
            exit(ErrorCode.CommonFail)

    def prepare_commits(self):
        '''
        获取各仓库的脚本提交记录
        '''
        # load commit infos
        self._load_commit_infos()
        # update url
        self._update_commits_url()

        # show
        # self._show_commits()

    def save_commits(self):
        self._save_commit_infos()

    def _show_commits(self):
        for k, v in self.commit_infos.items():
            print(f"---------{k}--------")
            for commit in v.logs:
                print(commit['revision'], commit['msg'])
                for p in commit["paths"]:
                    print(f"-----------{p['path']}")

    def _get_commit_info_file_path(self):
        return os.path.join(self.project_cmake_checker_root_path, self.commit_info_file)

    def _load_commit_infos(self):
        info_file_path = self._get_commit_info_file_path()
        with open(info_file_path, 'r') as fp:
            data = json.load(fp)
        for item in data:
            commit_info = RepoCommitInfo()
            commit_info.from_dict(item)
            self.commit_infos[commit_info.name] = commit_info
            self.priority_commit_infos.append(commit_info)

        self.priority_commit_infos = sorted(self.priority_commit_infos, key=lambda info: info.priority)

    def _save_commit_infos(self):
        info_file_path = self._get_commit_info_file_path()

        data = []
        for k, v in self.commit_infos.items():
            data.append(v.to_dict())

        with open(info_file_path, 'w') as fp:
            json.dump(data, fp)

    def _update_commits_url(self):
        self.commit_infos['mlproj2017'].repo_url = f"{self.main_branch_url}/Assets"
        self.commit_infos['mlclientcode'].repo_url = self.client_code_branch_url
        self.commit_infos['mlplugin'].repo_url = self.ml_plugin_branch_url

    def update_commits(self):
        for k, v in self.commit_infos.items():
            v.latest_revision = RepositoryUtil.get_last_revision(v.repo_url)
            v.step_index = -1
            self._get_script_commit_logs(v)

    @mini_profile()
    def _get_script_commit_logs(self, commit_info: RepoCommitInfo):
        '''
        一次性获取一个目录，再分析，比多个目录分别获取快。
        '''
        commit_info.logs = []
        if commit_info.latest_revision > commit_info.revision:
            logs = RepositoryUtil.get_logs(commit_info.repo_url, commit_info.revision + 1, commit_info.latest_revision)
            self._get_commits_from_log(logs, commit_info.logs)

    @staticmethod
    def _get_commits_from_log(logs, commits):
        for log in logs:
            # 忽略掉CI的提交
            if '[CI]' in log['msg']:
                continue
            # a_datetime = datetime.strptime(log['datetime'], '%Y-%m-%d %H:%M:%S')
            # b_datetiem = datetime.strptime("2024-04-16 00:00:00", '%Y-%m-%d %H:%M:%S')
            # if a_datetime>b_datetiem:
            #     continue
            if log["paths"]:
                for path_info in log["paths"]:
                    file = path_info["path"]
                    if file.endswith(".cs"):
                        commits.append(log)
                        break

    def reset_project_revision(self):
        for k, v in self.commit_infos.items():
            for relative_path in v.paths:
                print(f"reset {relative_path}->{v.revision}")
                full_path = os.path.join(self.project_root_path, relative_path)
                RepositoryUtil.update_path_depth(full_path, revision=v.revision)

    def sync_commit_info_revisions(self, success):
        for k, v in self.commit_infos.items():
            if success:
                # 如果最后编译通过，则递进所有提交号到最新
                v.revision = v.latest_revision
            elif v.success_revision == -1:
                # 没有成功提交记录
                if len(v.logs) == 0:
                    # 没有代码提交，忽略中间其他提交，直接保存到最新的记录号
                    v.revision = v.latest_revision
            else:
                # 使用成功的提交记录
                v.revision = v.success_revision
        self.save_commits()

    def upload_configs(self):
        RepositoryUtil.commit(os.path.join(self.project_cmake_checker_root_path, "config"),
                              "[CI]ProjectCompileChecker ")

    def get_revision_range(self):
        start_revisions = {}
        end_revisions = {}
        for k, v in self.commit_infos.items():
            if len(v.logs) > 0:
                start_revisions[k] = v.logs[0]['revision']
                end_revisions[k] = v.logs[-1]['revision']
            else:
                start_revisions[k] = v.revision
                end_revisions[k] = v.latest_revision
        return start_revisions, end_revisions


class BuildResult:
    def __init__(self, build_config, log, success=False):
        self.build_config = build_config
        self.log = log
        self.success = success

        self._error_lines = None

    def get_error_lines(self):
        if self._error_lines is not None:
            return self._error_lines

        self._error_lines = []
        lines = self.log.split("\n")
        for line in lines:
            line = line.strip()
            if not line:
                continue
            if ' error ' in line:
                self._error_lines.append(line)
        self._error_lines = sorted(self._error_lines)
        return self._error_lines

    def get_error(self):
        return '\n'.join(self.get_error_lines())


class CompileChecker:
    SEND_MESSAGE_TOOL = "Tools/MufGetAuthorsTools/AutoGetSvnAuthors.exe"
    CMAKE_TOOLS_PATH = os.environ.get('CMAKE_TOOLS_PATH') \
        if 'CMAKE_TOOLS_PATH' in os.environ else "D:/tools/CMake-3.23.2/bin"
    REPORT_TABLE = 'ci.ci_ml_dll_build_report'
    ERROR_DATA_FILE_PATH = "config/error_data.json"
    DATETIME_FORMAT = '%Y-%m-%d %H:%M:%S'

    def __init__(self, project_client: ProjectClient, check_output,
                 robot_key=None, clean_cache=True, need_commit_conf=True, debugging=False):
        self.project_client = project_client
        self.check_output = os.path.join(check_output, self.project_client.branch_version)
        self.clean_cache = clean_cache
        self.need_commit_conf = need_commit_conf

        self.steps_info: list[BuildStepInfo] = []
        self.step_index = -1
        self.one_step_results: list[BuildResult] = []

        # 检查到的错误步骤
        self.error_steps: list[BuildStepInfo] = []

        self.wait_repair_steps: list[BuildStepInfo] = []

        self.error_authors: list[str] = []

        # json dict
        self.error_data = {}

        self.log_temp_path = os.path.join(self.project_client.project_cmake_checker_root_path, "logs")

        self.robot_key = robot_key
        self.send_message_tool_full_path = os.path.join(
            self.project_client.project_cmake_checker_root_path, self.SEND_MESSAGE_TOOL)

        if os.path.exists(self.CMAKE_TOOLS_PATH):
            self.cmake_full_path = os.path.join(self.CMAKE_TOOLS_PATH, "cmake.exe")
        else:
            self.cmake_full_path = "cmake"

        self.start_time = None
        self.debugging = debugging

    @mini_profile()
    def start(self):
        self.start_time = datetime.now()
        self._prepare()
        self._check()

    def _prepare(self):
        self.project_client.prepare_build_project(not self.debugging)
        self.project_client.prepare_commits()
        self.project_client.update_commits()
        self.project_client.reset_project_revision()

        # 获取上次的错误信息
        self._load_error_data()

        if not os.path.exists(self.check_output):
            os.makedirs(self.check_output)

        # 准备代码仓库工程
        if self.clean_cache:
            self.project_client.clean_cache()
            PathUtil.clean_tree(self.log_temp_path)

        if not os.path.exists(self.log_temp_path):
            os.makedirs(self.log_temp_path)

    def _check(self):
        # 按时间排序所有提交
        self.create_step_infos()
        # 所有仓库回到初始位置

        while True:
            if not self._check_step():
                break
        # 更新结束，处理错误的提交记录
        success = self._parse_steps_results()

        # 处理带修护列表
        self._parse_left_wait_repair_steps()
        # 保存错误信息。如果没有错误信息是清空
        self._save_error_data()

        # 再设置commit_info的revision 最后检查的revision不一定是仓库中最新的
        self.project_client.sync_commit_info_revisions(success)
        # commit.TODO 准备分支和提交结果分离
        if self.need_commit_conf:
            self.project_client.upload_configs()
        # 上报到数据收集平台
        self._report_check_result(success)

    def create_step_infos(self):
        self.steps_info = []
        for k, commit_info in self.project_client.commit_infos.items():
            if commit_info.logs:
                for log in commit_info.logs:
                    step_info = BuildStepInfo(commit_info, log)
                    self.steps_info.append(step_info)
        # sort by time
        self.steps_info = sorted(self.steps_info,
                                 key=lambda item: datetime.strptime(item.repo_log["datetime"],
                                                                    self.DATETIME_FORMAT).timestamp())
        print(f"total steps {len(self.steps_info)}")
        # show
        for step_info in self.steps_info:
            print(step_info.commit_info.name, step_info.repo_log["revision"], step_info.repo_log["datetime"])

    def get_current_step_info(self):
        if self.step_index >= len(self.steps_info):
            # 全部处理完成
            return None

        return self.steps_info[self.step_index]

    def next_step(self):
        self.step_index += 1
        print(f"next_commit:{self.step_index + 1}/{len(self.steps_info)}")

        step_info = self.get_current_step_info()
        if not step_info:
            print(f"can't get commit log step:{self.step_index}")
            return None
        print(
            f"commit log:{step_info.commit_info.name},{step_info.repo_log['revision']},{step_info.repo_log['datetime']}")

        for relative_path in step_info.commit_info.paths:
            full_path = os.path.join(self.project_client.project_root_path, relative_path)
            print(f"update project path {full_path}->{step_info.repo_log['revision']}")
            RepositoryUtil.update_path_depth(full_path, revision=step_info.repo_log['revision'])

        return step_info

    def next_step2(self):
        for commit_info in self.project_client.priority_commit_infos:
            print(f"next:{commit_info.name}")
            if commit_info.is_end():
                print(f"end:{commit_info.name}")

                continue
            commit_info.next_step()
            log = commit_info.get_current_log()
            for relative_path in commit_info.paths:
                full_path = os.path.join(self.project_client.project_root_path, relative_path)
                RepositoryUtil.update_path_depth(full_path, revision=log['revision'])

    @mini_profile()
    def _check_step(self):
        step_info = self.next_step()
        if not step_info:
            return False

        # 生成vs工程
        if not self._gen_csharp_projects():
            print("Error:generate csharp project error")
            exit(1)
        self._backup_csharp_project(step_info)
        self._build_project_all(step_info)
        # check result
        if self._check_build_result(step_info):
            # 编译通过,提升对应的提交号
            print(f"check succes! {step_info.commit_info.name}.{step_info.repo_log['revision']}")
            # 保存成功的提交记录
            step_info.commit_info.success_revision = step_info.repo_log["revision"]
        else:
            # 编译失败，记录失败信息。这里只记录，不做错误分析
            print(f"check fail! {step_info.commit_info.name}.{step_info.repo_log['revision']}")
            self.error_steps.append(step_info)
            # 编译失败情况：
            # 1. 首次失败。
            # 2. 别人失败没有修，后续人继续编译出错。这种情况，根据报错信息来判断是不是后续人提交的新错误，如果是新错误，则二个人都通知。
        return True

    @mini_profile()
    def _gen_csharp_projects(self):
        cmd = f"{self.cmake_full_path} -S . -B build"
        ret = CmdUtil.exec(cmd, cwd=self.project_client.project_cmake_checker_root_path)
        self._check_cmd_ret(ret)
        return ret.returncode == 0

    def _backup_csharp_project(self, step_info):
        backup_to = os.path.join(self.project_client.project_cmake_checker_root_path, "backup")
        if not os.path.exists(backup_to):
            os.makedirs(backup_to)

        backup_to = os.path.join(backup_to, step_info.get_unique_key())
        build_path = os.path.join(self.project_client.project_cmake_checker_root_path, 'build')
        shutil.copytree(build_path, backup_to, dirs_exist_ok=True)

    @mini_profile()
    def _build_project_all(self, step_info: BuildStepInfo):
        self.one_step_results.clear()
        self._build_project("WindowsEditor", "Assembly-CSharp-Editor")
        self._build_project("WindowsRuntime", "Assembly-CSharp-NormalScripts")
        self._build_project("AndroidEditor", "Assembly-CSharp-Editor")
        self._build_project("AndroidRuntime", "Assembly-CSharp-NormalScripts")
        self._build_project("IosEditor", "Assembly-CSharp-Editor")
        self._build_project("IosRuntime", "Assembly-CSharp-NormalScripts")
        step_info.build_results.extend(self.one_step_results)

    @mini_profile()
    def _build_project(self, config, target):
        cmd = f"{self.cmake_full_path} --build build --config {config} --target {target}"
        ret = CmdUtil.exec(cmd, cwd=self.project_client.project_cmake_checker_root_path)
        if ret.returncode == 0:
            build_result = BuildResult(config, CmdUtil.decode_pipe_output(ret.stdout), True)
        else:
            build_result = BuildResult(config,
                                       CmdUtil.decode_pipe_output(ret.stderr if ret.stderr else ret.stdout),
                                       False)

        self.one_step_results.append(build_result)

    def _check_build_result(self, step_info):
        success = True
        for check_result in self.one_step_results:
            if not check_result.success:
                success = False
                print(f'check {check_result.build_config} error')
                # 把错误结果加入到步骤表里
                step_info.fail_build_results[check_result.build_config] = check_result

            self._save_build_log(check_result, step_info)
        return success

    def _parse_build_result(self, step_info):
        for check_result in self.one_step_results:
            self._save_build_log(check_result, step_info)

    def _get_step_log_folder(self, step_info: BuildStepInfo):
        return os.path.join(self.log_temp_path,
                            f"{step_info.commit_info.name}_{step_info.repo_log['revision']}")

    def _save_build_log(self, check_result: BuildResult, step_info: BuildStepInfo):
        log_file_parent_path = os.path.join(self.log_temp_path, step_info.get_unique_key())
        if not os.path.exists(log_file_parent_path):
            os.makedirs(log_file_parent_path)

        # build log
        log_file_name = f"{check_result.build_config}.txt"
        log_output_file_path = os.path.join(log_file_parent_path, log_file_name)
        with open(log_output_file_path, "w", encoding="utf8") as fp:
            fp.write(check_result.log)

        # error log
        if not check_result.success:
            log_output_file_path = log_output_file_path.replace(".txt", ".error.txt")
            with open(log_output_file_path, "w", encoding="utf8") as fp:
                fp.write(check_result.get_error())

    def _parse_steps_results(self):
        all_error_lines = {}

        for step_info in self.steps_info:
            if step_info.is_success():
                all_error_lines.clear()
                # 清空所有error数据。
                self.error_data.clear()
                # 清空所有待修复错误。
                self.wait_repair_steps.clear()
                self._do_report_step(step_info, True)
            else:
                print(f"Find Error:{step_info.commit_info.name}-")
                print(f"{step_info.repo_log['revision']},{step_info.repo_log['datetime']},"
                      f"{step_info.repo_log['author']},{step_info.repo_log['msg']}")

                if len(all_error_lines) == 0:
                    # 第一个失败，真的是失败。
                    print(f"first fail: "
                          f"{step_info.commit_info.name},"
                          f"{step_info.repo_log['revision']},"
                          f"{step_info.repo_log['author']}")
                    self._do_step_fail(step_info)
                    for build_config, v in step_info.fail_build_results.items():
                        all_error_lines[build_config] = set(v.get_error_lines())
                else:
                    have_new_error = self._check_and_set_step_error_is_new(step_info, all_error_lines)
                    # 如果后面编译失败的结果和前一个一样，则认为这次的失败，不是真的失败。
                    if have_new_error:
                        print(f"new fail: "
                              f"{step_info.commit_info.name}."
                              f"{step_info.repo_log['revision']}."
                              f"{step_info.repo_log['author']}")
                        self._do_step_fail(step_info)
                    else:
                        print(f"not fail: "
                              f"{step_info.commit_info.name}."
                              f"{step_info.repo_log['revision']}."
                              f"{step_info.repo_log['author']}")
                        self._check_fixed_wait_repair_steps(step_info)
                        self._do_report_step(step_info, True)

        print(f"wait repair step count: {len(self.wait_repair_steps)}")
        print(f"error data count: {len(self.error_data)}")
        print(f"all error lines: {len(all_error_lines)}")
        return len(all_error_lines) == 0

    @staticmethod
    def _check_and_set_step_error_is_new(step_info, all_error_lines):
        have_new_error = False
        # 如果后面编译失败的结果和前一个一样，则认为这次的失败，不是真的失败。
        for build_config, v in step_info.fail_build_results.items():
            if build_config not in all_error_lines:
                # 编译选项不存在，则之前没有出现过这个选项的错误。产生新的错误
                all_error_lines[build_config] = set(v.get_error_lines())
                have_new_error = True
            else:
                # 检查错误行在不在之前的错误里
                for error_line in v.get_error_lines():
                    if error_line not in all_error_lines[build_config]:
                        have_new_error = True
                        all_error_lines[build_config].add(error_line)
        return have_new_error

    def _get_error_data_full_path(self):
        return os.path.join(self.project_client.project_cmake_checker_root_path,
                            self.ERROR_DATA_FILE_PATH)

    def _load_error_data(self):
        error_data_path = self._get_error_data_full_path()

        if not os.path.exists(error_data_path):
            print(f"can't find error data file:{error_data_path}")
            return

        with open(error_data_path, "r", encoding="utf8") as fp:
            self.error_data = json.load(fp)

    def _save_error_data(self):
        error_data_path = self._get_error_data_full_path()
        with open(error_data_path, "w", encoding="utf8") as fp:
            json.dump(self.error_data, fp)

    def _get_commit_error_data(self, step_info):
        if len(self.error_data) == 0:
            return None

        commit_error_data = None
        repo_name = step_info.get_repo_name()
        if repo_name in self.error_data:
            revision_str = str(step_info.get_commit_revision())
            if revision_str in self.error_data[repo_name]:
                commit_error_data = self.error_data[repo_name][revision_str]
        return commit_error_data

    def _remove_commit_error_data(self, step_info):
        if len(self.error_data) == 0:
            return

        repo_name = step_info.get_repo_name()
        if repo_name not in self.error_data:
            return

        revision_str = str(step_info.get_commit_revision())
        if revision_str not in self.error_data[repo_name]:
            return

        del self.error_data[repo_name][revision_str]

    def _add_commit_error_data(self, step_info):
        repo_name = step_info.get_repo_name()
        if repo_name not in self.error_data:
            self.error_data[repo_name] = {}

        revision_str = str(step_info.get_commit_revision())
        commit_error_data = None
        if revision_str in self.error_data[repo_name]:
            commit_error_data = self.error_data[repo_name][revision_str]
        else:
            commit_error_data = {}
            self.error_data[repo_name][revision_str] = commit_error_data

        commit_error_data["last_check_time"] = datetime.now().strftime(self.DATETIME_FORMAT)
        # 是否需要把错误信息加进去，根据检查策略。
        # 如果只检查一次，则要保存详细的错误信息，来判断这个提交的错误是否修复。
        # 如果重利检查，则不用存错误，错误信息通过检查获得
        errors = {}
        for build_config, v in step_info.fail_build_results.items():
            errors[build_config] = list(v.get_error_lines())
        commit_error_data["errors"] = errors

    def _check_fixed_wait_repair_steps(self, current_step_info):
        all_error_lines = {}
        for build_config, v in current_step_info.fail_build_results.items():
            all_error_lines[build_config] = set(v.get_error_lines())

        repaired_steps = []
        for step_info in self.wait_repair_steps:
            have_error = False
            for build_config, v in step_info.fail_build_results.items():
                if build_config in all_error_lines:
                    # 检查错误还在不在
                    for error_line in v.get_error_lines():
                        if error_line in all_error_lines[build_config]:
                            # 错误还在，还没有修复
                            have_error = True
                            break
                if have_error:
                    break

            if not have_error:
                repaired_steps.append(step_info)

        for step_info in repaired_steps:
            # 已经修复。则从待修复列表和错误数据中删除
            self.wait_repair_steps.remove(step_info)
            self._remove_commit_error_data(step_info)

    def _parse_left_wait_repair_steps(self):
        # 剩余没有修复，继续通知。TODO 通知使用间隔
        for step_info in self.wait_repair_steps:
            print(f"error not repair:{step_info.get_repo_name()}.{step_info.get_commit_revision()}")
            self._do_commit_fail(step_info)
            self._do_report_step(step_info, False)

    def _do_step_fail(self, step_info):
        # 检查是不是已经检测过,如果检查过，则表示待修复状态, 根据处理结果，如果已经修复，则不发送信息，否则继续发错误信息。
        commit_error_data = self._get_commit_error_data(step_info)
        # 加入待修复列表
        self.wait_repair_steps.append(step_info)
        # 加入错误列表
        self._add_commit_error_data(step_info)

        if not commit_error_data:
            # 没有当前的提交对应的错误信息
            self._do_commit_fail(step_info)
            self._do_report_step(step_info, False)
            return

        print(f"not need report check repair {step_info.get_repo_name()}.{step_info.get_commit_revision()}")

    def _do_commit_fail(self, step_info):
        print(f"do commit failed {step_info.commit_info.name}.{step_info.repo_log['revision']}")

        self.error_authors.append(step_info.repo_log['author'])
        print("copy build fail log ")
        step_log_output_folder = self._get_step_log_folder(step_info)
        dist_log_output_folder = os.path.join(self.check_output, os.path.basename(step_log_output_folder))
        print(f"copy {step_log_output_folder}->{dist_log_output_folder}")
        if not self.debugging:
            shutil.copytree(step_log_output_folder, dist_log_output_folder, dirs_exist_ok=True)
            # disk backup
            CommonUtil.try_backup_to_remote_disk_auto(dist_log_output_folder)

        # send message
        # get relative path
        dist_log_output_folder = dist_log_output_folder.replace("\\", "/")
        pos = dist_log_output_folder.find("/")
        if pos > -1:
            relative_path = dist_log_output_folder[pos + 1:]
        else:
            relative_path = ""
        url = f"http://{HttpDomains.MOBA_GAME}/{relative_path}"

        brief_log_error_str = step_info.get_brief_error_str()

        message = f"详情:{url}\n错误:{brief_log_error_str}"

        if self.debugging:
            print(f"error msg: {message}")

        cmd = f'{self.send_message_tool_full_path}' \
              f' --svnUrl "{step_info.commit_info.repo_url}" --feishuToken {self.robot_key}' \
              f' --startRevision {step_info.repo_log["revision"]} --endRevision {step_info.repo_log["revision"]}' \
              f' --message "{message}"'
        try:
            if not self.debugging:
                CmdUtil.run(cmd, cwd=os.path.dirname(self.send_message_tool_full_path))
        except Exception as e:
            print(f"notify error:{e}")

    def _do_report_step(self, step_info, success):
        print(f"do report step {step_info.commit_info.name}.{step_info.repo_log['revision']} success:{success}")

    def _report_check_result(self, success):
        print(f"report check result success: {success}")

        start_revisions, end_revisions = self.project_client.get_revision_range()

        start_revision_str = ""
        for k, v in start_revisions.items():
            start_revision_str += f"{k}:{v};"
        start_revision_str = start_revision_str[0:-1]

        end_revision_str = ""
        for k, v in end_revisions.items():
            end_revision_str += f"{k}:{v};"
        end_revision_str = end_revision_str[0:-1]

        all_error_str = ""
        for error_step in self.error_steps:
            all_error_str += error_step.get_all_error_str()

        try:
            data = {
                "buildid": str(os.environ.get("BUILD_NUMBER")),
                "branchname": self.project_client.branch_version,
                "result": 1 if success else 0,
                "starttime": self.start_time.strftime(self.DATETIME_FORMAT),
                "endtime": datetime.now().strftime(self.DATETIME_FORMAT),
                "startrevision": start_revision_str,
                "endrevision": end_revision_str,
                "authors": ";".join(self.error_authors),
                "stack": all_error_str
            }
            if not self.debugging:
                connector = CISQLConnector(debugging=self.debugging)
                connector.connect()
                connector.insert_table(self.REPORT_TABLE)

                for k, v in data.items():
                    connector.update_col(k, v)
                connector.commit_insert()

                connector.close()
        except Exception as e:
            print(f"report mysql error:{e}")

    @staticmethod
    def _check_cmd_ret(ret):
        if ret.returncode != 0:
            print(
                f"cmake gen project error {ret.returncode} std:{CmdUtil.decode_pipe_output(ret.stdout)} "
                f"err:{CmdUtil.decode_pipe_output(ret.stderr)}")


def main():
    arg_parser = argparse.ArgumentParser()
    arg_parser.add_argument("-w", "--workspace", default=BuildPaths.AUTOBUILD_ROOT)
    arg_parser.add_argument("-pp", "--project_prefix", default="mobaclient.cc")
    arg_parser.add_argument("-bv", "--branch_version", default="trunk")
    arg_parser.add_argument("-o", "--output", default="Z:/BuildLog/CompileCheck")
    arg_parser.add_argument("-rk", "--robot_key", default="cd1240a4-2ba4-41a0-8a3f-267a2c6c215e")
    arg_parser.add_argument("-cc", "--clean_cache", default=True,
                            type=lambda x: (str(x).lower() in ['true', '1', 'yes']))

    arg_parser.add_argument("-ncc", "--need_commit_conf", default=True,
                            type=lambda x: (str(x).lower() in ['true', '1', 'yes']))

    arg_parser.add_argument("-d", "--debug", default=False,
                            type=lambda x: (str(x).lower() in ['true', '1', 'yes']))

    args = arg_parser.parse_args()

    project_client = ProjectClient(args.workspace, args.project_prefix, args.branch_version)
    project_client.setup()

    cc = CompileChecker(project_client=project_client, check_output=args.output, robot_key=args.robot_key,
                        clean_cache=args.clean_cache, need_commit_conf=args.need_commit_conf, debugging=args.debug)
    cc.start()


if __name__ == '__main__':
    main()
