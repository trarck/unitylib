
namespace YH.Excel.Data
{
    public enum ExcelDataType
    {
        Object,//未知
        Int,
        Float,
        String,
        Boolean,
        Array,
        Dictionary,
        List,
        Long,
        Double,
        //暂时不支持Enum，涉及动态创建Enum
        Custom=10000
    }
}