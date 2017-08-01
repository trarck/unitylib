#coding=utf-8

import platform
import time
import fileinput
import subprocess
import os
import sys
import thread
import time
import tail



def tail_thread(tail_file):

    print "wait for tail file ... %s" % tail_file

    while True:
        if os.path.exists(tail_file):
            print "Start tail file..... %s" % tail_file
            break

    t = tail.Tail(tail_file)
    t.register_callback(unity_log_tail)
    t.follow(s=1)

def unity_log_tail(txt):
    print(txt)

def build(args):
    """
    call unity process to build
    """
    if '-batchmode' not in args:
        args.append('-batchmode')
    
    if '-nographics' not in args:
        args.append('-nographics')
    
    if '-quit' not in args:
        args.append('-quit')
        
    project_path=''
    if '-projectPath' in args:
        projectPathValueIndex=args.index('-projectPath')+1
        project_path=fullpath(args[projectPathValueIndex])
        args[projectPathValueIndex]=project_path
    else:
        print 'not find project path'
    
    log_path=''
    if '-logFile' in args:
        logFileValueIndex=args.index('-logFile')+1
        log_path=fullpath(args[logFileValueIndex])
        args[logFileValueIndex]=log_path
    else:
        log_path=fullpath(os.path.join(project_path, '__kellylog.txt'))#fullpath('__kellylog.txt')#
        args.extend(['-logFile', log_path])
    
    build_cmd = args
    print 'Unity running ....'

    if os.path.exists(log_path):
        os.remove(log_path)
        print 'delete %s' % log_path

    # new thread to tail log file
    thread.start_new_thread(tail_thread, (log_path, ))

    process = subprocess.Popen(
        build_cmd, stdout=subprocess.PIPE, stderr=subprocess.STDOUT,cwd=project_path
    )

    while True:
        out = process.stdout.read(1)
        if out == '' and process.poll() != None:
            break
        if out != '':
            sys.stdout.write("[Unity process console output]: " + out)
            sys.stdout.flush()

    time.sleep(5)
    print 'done!'

def fullpath(path):
    return os.path.abspath(os.path.expanduser(path))

if __name__ == '__main__':
    args = sys.argv[1:]
    build(args)