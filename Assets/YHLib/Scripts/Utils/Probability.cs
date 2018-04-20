using UnityEngine;
using System.Collections.Generic;


namespace YH
{
	public class ProbabilityConfigItem
	{
		//概率
		public float probability;

		//产出
		public object data;
	}

	//格式化的配置表
	public class ProbabilityFormatedItem
	{
		//开始概率
		public float from;

		//结束概率
		public float to;

		//配置表中的位置
		public int index;
	}

	/// <summary>
	/// 概率计算
	/// 二种概率
	/// 1.单个结束,总的概率是100
	/// 2.多个结果，每个项单独计算
	/// </summary>
	public class Probability
	{
		//配置表
		List<ProbabilityConfigItem> m_ConfigTable;

		//格式化的概率表
		List<ProbabilityFormatedItem> m_FormatedTable;

		//基础概率100 || 1.0。用于取得多个结果
		float m_BaseProbability;

		//总概率
		float m_TotalProbability;

		bool m_Dirty = true;

		public Probability()
		{
			m_ConfigTable= new List<ProbabilityConfigItem>();
		}

		public void AddProbability(float probability,object data)
		{
			ProbabilityConfigItem item = new ProbabilityConfigItem();
			item.probability = probability;
			item.data = data;
			m_ConfigTable.Add(item);
			m_Dirty = true;
		}

		public virtual float Random(float max)
		{
			//[0,max]
			return UnityEngine.Random.Range(0, max);
		}

		void FormatConfigTable()
		{
			if (!m_Dirty) return;

			m_Dirty = false;

			if (m_FormatedTable == null)
			{
				m_FormatedTable = new List<ProbabilityFormatedItem>();
			}
			else
			{
				m_FormatedTable.Clear();
			}

			ProbabilityFormatedItem item = null;
			float totalProbability = 0;
			float last = 0;

			for (int i = 0; i < m_ConfigTable.Count; ++i)
			{
				last = totalProbability;
				totalProbability += m_ConfigTable[i].probability;

				item = new ProbabilityFormatedItem();
				item.from = last;
				item.to = totalProbability;
				item.index = i;

				m_FormatedTable.Add(item);
			}

			m_TotalProbability = totalProbability;
		}

		public object GetResult()
		{
			FormatConfigTable();
			ProbabilityFormatedItem item = null;

			float rand = Random(m_TotalProbability);

			for (int i = 0; i < m_FormatedTable.Count; ++i)
			{
				item = m_FormatedTable[i];

				if (rand > item.from && rand <= item.to)
				{
					//命中
					return m_ConfigTable[item.index].data;
				}
			}

			return null;
		}

		public List<object> GetMultiResult()
		{
			ProbabilityConfigItem item = null;

			List<object> result = new List<object>();

			for (int i = 0; i < m_ConfigTable.Count; ++i)
			{
				float rand = Random(m_BaseProbability);
				item = m_ConfigTable[i];

				if (rand <= item.probability)
				{
					//命中
					result.Add(item.data);
				}
			}

			return result;
		}

		public List<ProbabilityConfigItem> configTable
		{
			set
			{
				m_ConfigTable = value;
				m_Dirty = true;
			}

			get
			{
				return m_ConfigTable;
			}
		}

		public List<ProbabilityFormatedItem> formatedTable
		{
			set
			{
				m_FormatedTable = value;
			}

			get
			{
				return m_FormatedTable;
			}
		}

		public float baseProbability
		{
			set
			{
				m_BaseProbability = value;
			}

			get
			{
				return m_BaseProbability;
			}
		}
	}
}