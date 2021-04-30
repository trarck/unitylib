public class BezierHelper
{

	private static void GetTwoPointsControlPoints(List<Vector3> points, ref List<Vector3> controlPoints)
	{
		Vector3 middle = (points[0] + points[1]) * 0.5f;
		controlPoints.Add(middle);
		controlPoints.Add(middle);
	}

	private static bool TryGetControlPointsOfStartEndMiddle(List<Vector3> points, ref List<Vector3> controlPoints)
	{
		if (points.Count < 3 || controlPoints.Count < 3)
		{
			return false;
		}

		//start
		controlPoints[0] = (points[0] + controlPoints[1]) * 0.5f;

		//end
		int pointLastIndex = points.Count - 1;
		int controlPointLastIndex = controlPoints.Count - 1;
		Vector3 controlPoint = (points[pointLastIndex] + controlPoints[controlPointLastIndex]) * 0.5f;
		controlPoints.Add(controlPoint);

		return true;
	}

	private static bool TryGetControlPointsOfStartEndCutXAxis(List<Vector3> points, ref List<Vector3> controlPoints)
	{
		if (points.Count < 3 || controlPoints.Count < 3)
		{
			return false;
		}

		//start
		//检查控制点有没有超出起点范围
		if (controlPoints[1].x < points[0].x)
		{
			float dx = Mathf.Abs(points[0].x - controlPoints[1].x);
			float sx = Mathf.Abs(controlPoints[1].x - points[1].x);

			Vector3 p = controlPoints[1] - points[1];
			controlPoints[1] = points[1] + p.normalized * p.magnitude * (1 - dx / sx);
		}

		controlPoints[0] = (points[0] + controlPoints[1]) * 0.5f;

		//end
		int pointLastIndex = points.Count - 1;
		int controlPointLastIndex = controlPoints.Count - 1;

		//检查控制点有没有超出起点范围
		if (controlPoints[controlPointLastIndex].x > points[pointLastIndex].x)
		{
			float dx = Mathf.Abs(points[pointLastIndex].x - controlPoints[controlPointLastIndex].x);
			float sx = Mathf.Abs(controlPoints[controlPointLastIndex].x - points[pointLastIndex - 1].x);

			Vector3 p = controlPoints[controlPointLastIndex] - points[pointLastIndex - 1];
			controlPoints[controlPointLastIndex] = points[pointLastIndex - 1] + p.normalized * p.magnitude * (1 - dx / sx);
		}
		Vector3 controlPoint = (points[pointLastIndex] + controlPoints[controlPointLastIndex]) * 0.5f;
		controlPoints.Add(controlPoint);

		return true;
	}


	private static bool TryGetControlPointsWithoutStartEndParallel(List<Vector3> points, float tension, ref List<Vector3> controlPoints)
	{
		if (points.Count < 3)
		{
			return false;
		}

		for (int i = 1; i < points.Count - 1; ++i)
		{
			Vector3 prev = points[i - 1];
			Vector3 next = points[i + 1];

			Vector3 Vpi = points[i] - prev;
			Vector3 Vin = next - points[i];

			Vector3 h = (next - prev).normalized;

			Vector3 controlPoint1 = points[i] - h * (Vpi.magnitude * tension);
			Vector3 controlPoint2 = points[i] + h * (Vin.magnitude * tension);

			controlPoints.Add(controlPoint1);
			controlPoints.Add(controlPoint2);
		}
		return true;
	}

	private static bool TryGetControlPointsWithoutStartEndTangent(List<Vector3> points, float tension, ref List<Vector3> controlPoints)
	{
		if (points.Count < 3)
		{
			return false;
		}

		for (int i = 1; i < points.Count - 1; ++i)
		{
			Vector3 prev = points[i - 1];
			Vector3 next = points[i + 1];

			Vector3 Vpi = points[i] - prev;
			Vector3 Vin = next - points[i];

			Vector3 h = (Vector3.Normalize(Vpi) + Vector3.Normalize(Vin)).normalized;

			Vector3 controlPoint1 = points[i] - h * (Vpi.magnitude * tension);
			Vector3 controlPoint2 = points[i] + h * (Vin.magnitude * tension);

			controlPoints.Add(controlPoint1);
			controlPoints.Add(controlPoint2);
		}
		return true;
	}

	/// <summary>
	/// 在平行线方向取控制点
	/// 计算快。
	/// 二个端点取平均值没什么大问题。
	/// </summary>
	/// <param name="points"></param>
	/// <param name="tension"></param>
	/// <returns></returns>

	public static List<Vector3> GetControlPointsParallel(List<Vector3> points, float tension)
	{
		if (points == null || points.Count < 2)
		{
			return null;
		}

		List<Vector3> controlPoints = new List<Vector3>((points.Count - 1) * 2);

		if (points.Count == 2)
		{
			GetTwoPointsControlPoints(points, ref controlPoints);
		}
		else
		{
			//增加一个空的点。处理端点时，不用移动数组。
			controlPoints.Add(Vector3.zero);

			if (TryGetControlPointsWithoutStartEndParallel(points, tension, ref controlPoints))
			{
				TryGetControlPointsOfStartEndMiddle(points, ref controlPoints);
			}
		}

		return controlPoints;
	}

	/// <summary>
	/// 在切线方向取控制点
	/// 更贴近真实。
	/// 计算稍微慢些，可以忽略。
	/// 二个端点的取控制点算法有待改进。特别是在统计图里，tension大些，会超出画图范围。
	/// </summary>
	/// <param name="points"></param>
	/// <param name="tension"></param>
	/// <returns></returns>

	public static List<Vector3> GetControlPointsTangent(List<Vector3> points, float tension)
	{
		if (points == null || points.Count < 2)
		{
			return null;
		}

		List<Vector3> controlPoints = new List<Vector3>((points.Count - 1) * 2);

		if (points.Count == 2)
		{
			GetTwoPointsControlPoints(points, ref controlPoints);
		}
		else
		{
			//增加一个空的点。处理端点时，不用移动数组。
			controlPoints.Add(Vector3.zero);

			if (TryGetControlPointsWithoutStartEndTangent(points, tension, ref controlPoints))
			{
				TryGetControlPointsOfStartEndCutXAxis(points, ref controlPoints);
			}
		}

		return controlPoints;
	}
}
