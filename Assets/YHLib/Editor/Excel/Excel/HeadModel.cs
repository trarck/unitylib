
namespace TK.Excel
{
    public class HeadModel
    {
        public int NameRow = 0;
        public int DataTypeRow = 1;
        public int DescriptionRow = 2;
        public int SideRow = 3;
        public int DataRow = 4;

        public HeadModel()
        {

        }

        public static HeadModel CreateSimpleModel()
        {
            HeadModel model = new HeadModel();
            model.DataRow = 2;
            model.DescriptionRow = -1;
            model.SideRow = -1;
            return model;
        }

        public static HeadModel CreateNormalModel()
        {
            HeadModel model = new HeadModel();
            model.DescriptionRow = 2;
            model.DataRow = 3;
            model.SideRow = -1;
            return model;
        }

        public static HeadModel CreateModel()
        {
            HeadModel model = new HeadModel();
            return model;
        }
    }

}