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
            m_ConditionOperators[(int)FindCondition.Operation.Equal] = new Equal();
            m_ConditionOperators[(int)FindCondition.Operation.NotEqual] = new NotEqual();
            m_ConditionOperators[(int)FindCondition.Operation.Less] = new Less();
            m_ConditionOperators[(int)FindCondition.Operation.LessEqual] = new LessEqual();
            m_ConditionOperators[(int)FindCondition.Operation.Big] = new Big();
            m_ConditionOperators[(int)FindCondition.Operation.BigEqual] = new BigEqual();
            m_ConditionOperators[(int)FindCondition.Operation.Contains] = new Contains();

            //m_ExpressionOperators[ModifyExpression.Operation.Set] = new Set();
            m_ExpressionOperators[(int)ModifyExpression.Operation.Add] = new Add();
            m_ExpressionOperators[(int)ModifyExpression.Operation.Sub] = new Sub();
            m_ExpressionOperators[(int)ModifyExpression.Operation.Mul] = new Mul();
            m_ExpressionOperators[(int)ModifyExpression.Operation.Div] = new Div();
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

        public  List<FindResult> Search(string searchPath,string filter,ClassInfo classInfo, List<BatchExpression> conditions)
        {
            if (string.IsNullOrEmpty(searchPath))
            {
                searchPath = "Assets";
            }

            if (classInfo == null)
            {
                //TODO
            }

            //Find Component
            return FindComponents(searchPath, filter, classInfo, conditions);
        }

        List<FindResult> FindComponents(string searchPath, string filter, ClassInfo classInfo, List<BatchExpression> conditions)
        {
            List<FindResult> results = new List<FindResult>();

            //convert conditions value type
            List<object> conditionsValue = new List<object>();
            for (int i = 0; i < conditions.Count; ++i)
            {
                object conditionValue = conditions[i].value;
                //这里必须从类里查找，条件可能是自定义的
                MemberInfo member = ReflectionUtils.GetMember(classInfo.type, conditions[i].name);
                if (member != null)
                {
                    Type conditionType = ReflectionUtils.GetFieldOrPropertyType(member);

                    if (conditionValue.GetType() != conditionType)
                    {
                        conditionValue = Convert.ChangeType(conditionValue, conditionType);
                    }
                    conditionsValue.Add(conditionValue);
                }
                else
                {
                    conditionsValue.Add(null);
                }
            }


            List<string> assets = FindAsset.FindAllAssets(searchPath, filter);

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
                            if (conditions != null && conditions.Count > 0)
                            {
                                for (int k = 0; k < conditions.Count; ++k)
                                {
                                    object conditionValue = conditionsValue[k];
                                    if (conditionValue == null)
                                    {
                                        continue;
                                    }

                                    BatchExpression condition = conditions[k];
                         
                                    MemberInfo member = ReflectionUtils.GetMember(classInfo.type, condition.name);

                                    if (member != null)
                                    {
                                        object fieldValue = ReflectionUtils.GetValue(member,insts[j]);

                                        if (m_ConditionOperators.ContainsKey(condition.op) && m_ConditionOperators[condition.op].Execute(fieldValue, conditionValue))
                                        {
                                            results.Add(new FindResult(assets[i] + ":" + HierarchyUtil.FullPath(insts[j].transform), insts[j]));
                                        }
                                    }
                                }
                            }
                            else
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

        int ModifyComponents(List<FindResult> results, ClassInfo classInfo, List<BatchExpression> expresstions)
        {
            int n = 0;

            //convert expressions value type
            //List<object> expressionsValue = new List<object>();
            //for (int i = 0; i < expresstions.Count; ++i)
            //{
            //    //get from class type
            //    FieldInfo field = classInfo.type.GetField(expresstions[i].field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            //    if (field != null)
            //    {
            //        object expressionValue = expresstions[i].value;

            //        if (expressionValue.GetType() != field.FieldType)
            //        {
            //            expressionValue = Convert.ChangeType(expressionValue, field.FieldType);
            //        }
            //        expressionsValue.Add(expressionValue);
            //    }
            //    else
            //    {
            //        expressionsValue.Add(null);
            //    }
            //}

            if (results != null && results.Count > 0 && expresstions != null && expresstions.Count > 0)
            {
                for (int i = 0; i < results.Count; ++i)
                {
                    FindResult result = results[i];

                    for (int j = 0; j < expresstions.Count; ++j)
                    {
                        BatchExpression expression = expresstions[j];

                        if (expression.value != null)
                        {
                            MemberInfo member = ReflectionUtils.GetMember(classInfo.type, expression.name);

                            if (member != null)
                            {
                                object expressionValue = expression.value;
                                Type memberType = ReflectionUtils.GetFieldOrPropertyType(member);
                                object memberValue = ReflectionUtils.GetValue(member, result.obj);

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
    }
}