using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace YH
{
    [Serializable]
    public class ClassInfo
    {
        public string className;
        public string[] fieldNames;
        public Type type;
    }

    [Serializable]
    public class FindCondition
    {
        public enum Operation
        {
            Equal,//=
            NotEqual,//!=
            Less,//<
            LessEqual,//<=
            Big,//>
            BigEqual,//>=
            Contains,//contain
        }

        public int fieldIndex = 0;
        public string field;
        public Operation op;
        public string value;//输入的时候统一字符串，在具体执行逻辑的时候进行转换。
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

    [Serializable]
    public class ModifyExpression
    {
        public enum Operation
        {
            Set,//=
            Add,//+
            Sub,//-
            Mul,//*
            Div,///
        }

        public int fieldIndex = 0;
        public string field;
        public Operation op;
        public string value;//输入的时候统一字符串，在具体执行逻辑的时候进行转换。
    }

    public class Batch
    {
        public ClassInfo findClassInfo=new ClassInfo();
        public List<FindResult> findResults=null;


        Dictionary<FindCondition.Operation, RelationalOperator> m_ConditionOperators = new Dictionary<FindCondition.Operation, RelationalOperator>();
        Dictionary<ModifyExpression.Operation, ValueOperator> m_ExpressionOperators = new Dictionary<ModifyExpression.Operation, ValueOperator>();

        public void Init()
        {
            InitOperators();
        }

        void InitOperators()
        {
            m_ConditionOperators[FindCondition.Operation.Equal] = new Equal();
            m_ConditionOperators[FindCondition.Operation.NotEqual] = new NotEqual();
            m_ConditionOperators[FindCondition.Operation.Less] = new Less();
            m_ConditionOperators[FindCondition.Operation.LessEqual] = new LessEqual();
            m_ConditionOperators[FindCondition.Operation.Big] = new Big();
            m_ConditionOperators[FindCondition.Operation.BigEqual] = new BigEqual();
            m_ConditionOperators[FindCondition.Operation.Contains] = new Contains();

            m_ExpressionOperators[ModifyExpression.Operation.Set] = new Set();
            m_ExpressionOperators[ModifyExpression.Operation.Add] = new Add();
            m_ExpressionOperators[ModifyExpression.Operation.Sub] = new Sub();
            m_ExpressionOperators[ModifyExpression.Operation.Mul] = new Mul();
            m_ExpressionOperators[ModifyExpression.Operation.Div] = new Div();
        }

        public void RefreshFindClassInfo(string className)
        {
            findClassInfo = GetClassInfo(className);
        }

        public static ClassInfo GetClassInfo(string className)
        {
            ClassInfo classInfo = null;

            if (!string.IsNullOrEmpty(className))
            {
                classInfo= new ClassInfo();
                classInfo.className = className;
                classInfo.type = ReflectionUtils.Instance.GetType(className);

                if (classInfo.type != null)
                {
                    FieldInfo[] fields = ReflectionUtils.Instance.GetSerializableFields(classInfo.type);
                    string[] names = new string[fields.Length + 1];
                    for (int i = 0; i < fields.Length; ++i)
                    {
                        names[i] = fields[i].Name;
                    }
                    names[fields.Length] = "Custom";
                    classInfo.fieldNames = names;
                }
                else
                {
                    classInfo.fieldNames = null;
                }
            }
            return classInfo;
        }

        public  List<FindResult> Search(string searchPath,string filter,ClassInfo classInfo, List<FindCondition> conditions)
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

        List<FindResult> FindComponents(string searchPath, string filter, ClassInfo classInfo, List<FindCondition> conditions)
        {
            List<FindResult> results = new List<FindResult>();

            //convert conditions value type
            List<object> conditionsValue = new List<object>();
            for (int i = 0; i < conditions.Count; ++i)
            {
                //get from class type
                FieldInfo field = classInfo.type.GetField(conditions[i].field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                {
                    object conditionValue = conditions[i].value;

                    if (conditionValue.GetType() != field.FieldType)
                    {
                        conditionValue = Convert.ChangeType(conditionValue, field.FieldType);
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
                                    FindCondition condition = conditions[k];
                                    FieldInfo field = classInfo.type.GetField(condition.field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                                    if (field != null)
                                    {
                                        object fieldValue = field.GetValue(insts[j]);
                                        object conditionValue = conditionsValue[k];

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

        public int Modify(List<ModifyExpression> expresstions)
        {
            return ModifyComponents(findResults, findClassInfo, expresstions);
        }

        public int Modify(List<ModifyExpression> expresstions, List<FindResult> results, ClassInfo classInfo)
        {
            return ModifyComponents(results, classInfo, expresstions);
        }

        int ModifyComponents(List<FindResult> results, ClassInfo classInfo, List<ModifyExpression> expresstions)
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

            if (results!=null && results.Count > 0 && expresstions!=null && expresstions.Count>0)
            {
                for (int j = 0; j < expresstions.Count; ++j)
                {
                    ModifyExpression expression = expresstions[j];
                    FieldInfo field = classInfo.type.GetField(expression.field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                    if (field != null)
                    {
                        for (int i = 0; i < results.Count; ++i)
                        {
                            FindResult result = results[i];
                            object fieldValue = field.GetValue(result.obj);
                            object expressionValue = expression.value;

                            if (expressionValue.GetType() != fieldValue.GetType())
                            {
                                expressionValue = Convert.ChangeType(expressionValue, fieldValue.GetType());
                            }

                            if (m_ExpressionOperators.ContainsKey(expression.op))
                            {
                                expressionValue=m_ExpressionOperators[expression.op].Execute(fieldValue, expressionValue);
                            }

                            field.SetValue(result.obj, expressionValue);
                            ++n;
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