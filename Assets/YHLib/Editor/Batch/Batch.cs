using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace YH
{
    public class AccessInfo
    {
        public MemberInfo member;

        public AccessInfo() { }

        public AccessInfo(MemberInfo member)
        {
            this.member = member;
        }

        public void SetValue(object obj,object value)
        {
            ReflectionUtils.SetValue(member, obj, value);
        }

        public object GetValue(object obj)
        {
            return ReflectionUtils.GetValue(member,obj);
        }

        public Type type
        {
            get{
                return ReflectionUtils.GetFieldOrPropertyType(member);
            }
        }

        public string name
        {
            get
            {
                return member != null ? member.Name : null;
            }
        }
    }

    [Serializable]
    public class ClassInfo
    {
        public string className;
        public Type type;
        public List<MemberInfo> accesses;

        //public string[] memberNames;
        //public Dictionary<string, Type> memberTypes;

        public string[] GetMemberNames(bool inhert=true)
        {
            if (accesses != null)
            {
                List<string> names = new List<string>();
                for(int i = 0; i < accesses.Count; ++i)
                {
                    if (inhert || accesses[i].DeclaringType==type) {
                        names.Add(accesses[i].Name);
                    }
                }
                names.Add("Custom");
                return names.ToArray();
            }
            return null;
        }

        public Type GetMemberType(string name)
        {
            if (accesses != null)
            {
                for (int i = 0; i < accesses.Count; ++i)
                {
                    if (accesses[i].Name == name)
                    {
                        return ReflectionUtils.GetFieldOrPropertyType(accesses[i]);
                    }
                }
            }
            return null;
        }
    }


    [Serializable]
    public class BatchExpression
    {
        public int index = 0;
        public string name;
        public int op;
        public object value;
        public Type type;
        public BatchExpressionGroup parent;
    }

    
    public class BatchExpressionGroup
    {
        public enum GroupType
        {
            Normal,//no group
            And,
            Or
        }
        public GroupType type=GroupType.Normal;
        public List<BatchExpression> expressions=new List<BatchExpression>();
        public List<BatchExpressionGroup> subGroups=new List<BatchExpressionGroup>();
        public BatchExpressionGroup parent;

        public void Clear()
        {
            expressions.Clear();
            subGroups.Clear();
        }

        public List<BatchExpression> GetNotNullExpressions()
        {
            List<BatchExpression> result = new List<BatchExpression>();

            for (int i = 0; i < expressions.Count; ++i)
            {
                if (expressions[i].value != null)
                {
                    result.Add(expressions[i]);
                }
            }
            return result;
        }
    }

    public enum FindOperation
    {
        Equal,//=
        NotEqual,//!=
        Less,//<
        LessEqual,//<=
        Big,//>
        BigEqual,//>=
        Contains,//contain
    }

    public enum ModifyOperation
    {
        Set,//=
        Add,//+
        Sub,//-
        Mul,//*
        Div,///
    }

    [Serializable]
    public class FindResult
    {
        public string path;
        public UnityEngine.Object obj;

        public FindResult(string path, UnityEngine.Object obj)
        {
            this.path = path;
            this.obj = obj;
        }
    }

    public class Batch
    {
        public ClassInfo findClassInfo=new ClassInfo();
        public List<FindResult> findResults=null;


        Dictionary<int, RelationalOperator> m_ConditionOperators = new Dictionary<int, RelationalOperator>();
        Dictionary<int, ValueOperator> m_ExpressionOperators = new Dictionary<int, ValueOperator>();

        public void Init()
        {
            InitOperators();
        }

        void InitOperators()
        {
            m_ConditionOperators[(int)FindOperation.Equal] = new Equal();
            m_ConditionOperators[(int)FindOperation.NotEqual] = new NotEqual();
            m_ConditionOperators[(int)FindOperation.Less] = new Less();
            m_ConditionOperators[(int)FindOperation.LessEqual] = new LessEqual();
            m_ConditionOperators[(int)FindOperation.Big] = new Big();
            m_ConditionOperators[(int)FindOperation.BigEqual] = new BigEqual();
            m_ConditionOperators[(int)FindOperation.Contains] = new Contains();

            //m_ExpressionOperators[ModifyOperation.Set] = new Set();
            m_ExpressionOperators[(int)ModifyOperation.Add] = new Add();
            m_ExpressionOperators[(int)ModifyOperation.Sub] = new Sub();
            m_ExpressionOperators[(int)ModifyOperation.Mul] = new Mul();
            m_ExpressionOperators[(int)ModifyOperation.Div] = new Div();
        }

        public void RefreshFindClassInfo(string className,bool inherit)
        {
            findClassInfo = GetClassInfo(className, inherit);
        }

        public static ClassInfo GetClassInfo(string className,bool inherit)
        {
            ClassInfo classInfo = null;

            if (!string.IsNullOrEmpty(className))
            {
                classInfo= new ClassInfo();
                classInfo.className = className;
                classInfo.type = ReflectionUtils.Instance.GetType(className);

                if (classInfo.type != null)
                {
                    Dictionary<string, Type> memberTypes = new Dictionary<string, Type>();
                    classInfo.accesses = ReflectionUtils.GetAccessableFieldAndProperties(classInfo.type, false);
                }
                else
                {
                    classInfo.accesses = null;
                }
            }
            return classInfo;
        }

        void FixExpressionsValue(ClassInfo classInfo,List<BatchExpression> expressions)
        {
            for (int j = 0; j < expressions.Count; ++j)
            {
                BatchExpression expression = expressions[j];

                if (expression.value != null)
                {
                    MemberInfo member = ReflectionUtils.GetMember(classInfo.type, expression.name);

                    if (member != null)
                    {
                        Type memberType = ReflectionUtils.GetFieldOrPropertyType(member);

                        if (expression.value.GetType() != memberType)
                        {
                            expression.value = Convert.ChangeType(expression.value, memberType);
                        }
                    }
                }
            }
        }

        void FixExpressionsValue(ClassInfo classInfo, BatchExpressionGroup group)
        {
            if (group != null)
            {
                FixExpressionsValue(classInfo, group.expressions);

                for (int j = 0; j < group.subGroups.Count; ++j)
                {
                    FixExpressionsValue(classInfo, group);
                }
            }
        }

        bool CheckConditions(ClassInfo classInfo, object obj, List<BatchExpression> conditions, bool isAny)
        {
            bool pass = true;

            if (conditions != null && conditions.Count > 0)
            {
                pass = !isAny;

                for (int k = 0; k < conditions.Count; ++k)
                {
                    BatchExpression condition = conditions[k];

                    MemberInfo member = ReflectionUtils.GetMember(classInfo.type, condition.name);

                    if (member != null)
                    {
                        object fieldValue = ReflectionUtils.GetValue(member, obj);
                        object conditionValue = condition.value;

                        if (m_ConditionOperators.ContainsKey(condition.op))
                        {
                            if (m_ConditionOperators[condition.op].Execute(fieldValue, conditionValue))
                            {
                                if (isAny)
                                {
                                    pass = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (!isAny)
                                {
                                    pass = false;
                                    break;
                                }
                            }

                        }
                    }
                }
            }
            return pass;
        }

        bool CheckConditions(ClassInfo classInfo, object obj, BatchExpressionGroup group)
        {
            bool pass = true;

            if (group != null && group.expressions.Count > 0)
            {
                pass=CheckConditions(classInfo, obj, group.expressions, group.type == BatchExpressionGroup.GroupType.Or);

                if (group.type == BatchExpressionGroup.GroupType.And)
                {
                    if (!pass)
                    {
                        return false;
                    }
                }
                else if (group.type == BatchExpressionGroup.GroupType.Or)
                {
                    if (pass)
                    {
                        return true;
                    }
                }

                for(int i = 0; i < group.subGroups.Count; ++i)
                {
                    pass = CheckConditions(classInfo, obj, group.subGroups[i]);

                    if (group.type == BatchExpressionGroup.GroupType.And)
                    {
                        if (!pass)
                        {
                            return false;
                        }
                    }
                    else if (group.type == BatchExpressionGroup.GroupType.Or)
                    {
                        if (pass)
                        {
                            return true;
                        }
                    }
                }
            }
            return pass;
        }

        public List<FindResult> FindComponents(string searchPath, string filter, ClassInfo classInfo, List<BatchExpression> conditions,bool isAny=true)
        {
            List<FindResult> results = new List<FindResult>();

            List<string> assets = FindAsset.FindAllAssets(searchPath, filter);

            FixExpressionsValue(classInfo, conditions);

            for (int i = 0; i < assets.Count; ++i)
            {
                GameObject gameObj = AssetDatabase.LoadAssetAtPath<GameObject>(assets[i]);

                if (gameObj != null)
                {
                    Component[] insts = gameObj.GetComponentsInChildren(classInfo.type);
                    if (insts != null && insts.Length > 0)
                    {
                        for (int j = 0; j < insts.Length; ++j)
                        {
                            if (CheckConditions(classInfo, insts[i], conditions, isAny))
                            {
                                results.Add(new FindResult(assets[i] + ":" + HierarchyUtil.FullPath(insts[j].transform), insts[j]));
                            }
                        }
                    }
                }
            }

            return results;
        }


        public List<FindResult> FindComponents(string searchPath, string filter, ClassInfo classInfo, BatchExpressionGroup root)
        {
            List<FindResult> results = new List<FindResult>();

            List<string> assets = FindAsset.FindAllAssets(searchPath, filter);

            FixExpressionsValue(classInfo, root);

            for (int i = 0; i < assets.Count; ++i)
            {
                GameObject gameObj = AssetDatabase.LoadAssetAtPath<GameObject>(assets[i]);

                if (gameObj != null)
                {
                    Component[] insts = gameObj.GetComponentsInChildren(classInfo.type);
                    if (insts != null && insts.Length > 0)
                    {
                        for (int j = 0; j < insts.Length; ++j)
                        {
                            if (CheckConditions(classInfo, insts[i], root))
                            {
                                results.Add(new FindResult(assets[i] + ":" + HierarchyUtil.FullPath(insts[j].transform), insts[j]));
                            }
                        }
                    }
                }
            }

            return results;
        }

        public int Modify(List<BatchExpression> expresstions)
        {
            return ModifyComponents(findResults, findClassInfo, expresstions);
        }

        public int Modify(List<BatchExpression> expresstions, List<FindResult> results, ClassInfo classInfo)
        {
            return ModifyComponents(results, classInfo, expresstions);
        }

        int ModifyComponents(List<FindResult> results, ClassInfo classInfo, List<BatchExpression> expressions)
        {
            int n = 0;

            if (results != null && results.Count > 0 && expressions != null && expressions.Count > 0)
            {
                for (int i = 0; i < results.Count; ++i)
                {
                    FindResult result = results[i];

                    for (int j = 0; j < expressions.Count; ++j)
                    {
                        BatchExpression expression = expressions[j];

                        if (expression.value != null)
                        {
                            MemberInfo member = ReflectionUtils.GetMember(classInfo.type, expression.name);

                            if (member != null)
                            {
                                object expressionValue = expression.value;                                
                                object memberValue = ReflectionUtils.GetValue(member, result.obj);
                                Type memberType = ReflectionUtils.GetFieldOrPropertyType(member);

                                if (expressionValue.GetType() != memberType)
                                {
                                    expressionValue = Convert.ChangeType(expressionValue, memberType);
                                }

                                if (m_ExpressionOperators.ContainsKey(expression.op))
                                {
                                    expressionValue = m_ExpressionOperators[expression.op].Execute(memberValue, expressionValue);
                                }

                                ReflectionUtils.SetValue(member, result.obj, expressionValue);
                                ++n;
                            }
                        }
                    }
                }
            }

            if (n > 0)
            {
                AssetDatabase.SaveAssets();
            }

            return n;
        }

        public List<FindResult> FindRefrences(string searchPath, string filter, string asset)
        {
            List<FindResult> results = new List<FindResult>();

            List<string> assets = FindAsset.FindAllAssets(searchPath, filter);

            for (int i = 0; i < assets.Count; ++i)
            {
                string[] deps = AssetDatabase.GetDependencies(assets[i]);
                if (ArrayUtility.Contains<string>(deps, asset))
                {
                    results.Add(new FindResult(assets[i], AssetDatabase.LoadAssetAtPath<GameObject>(assets[i])));
                }
            }

            return results;
        }

        public List<FindResult> FindResources(string searchPath, string filter, ClassInfo classInfo, List<BatchExpression> conditions,bool isAny=true)
        {
            List<FindResult> results = new List<FindResult>();

            List<string> assets = FindAsset.FindAllAssets(searchPath, filter);

            FixExpressionsValue(classInfo, conditions);

            for (int i = 0; i < assets.Count; ++i)
            {
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(assets[i],classInfo.type);

                if (obj!=null)
                {
                    if (CheckConditions(classInfo, obj, conditions, isAny))
                    {
                        results.Add(new FindResult(assets[i], obj));
                    }
                }
            }

            return results;
        }

        public List<FindResult> FindResources(string searchPath, string filter, ClassInfo classInfo, BatchExpressionGroup root)
        {
            List<FindResult> results = new List<FindResult>();

            List<string> assets = FindAsset.FindAllAssets(searchPath, filter);

            FixExpressionsValue(classInfo, root);

            for (int i = 0; i < assets.Count; ++i)
            {
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(assets[i], classInfo.type);

                if (obj != null)
                {
                    if (CheckConditions(classInfo, obj, root))
                    {
                        results.Add(new FindResult(assets[i], obj));
                    }
                }
            }

            return results;
        }
    }
}