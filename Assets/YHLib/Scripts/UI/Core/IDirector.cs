namespace YH.UI
{
    public interface IDirector
    {
        void Push(string panelAsset, object data = null);
        void Pop();
        void Replace(string panelAsset, object data = null);
        void PopToStackLevel(int level);
        void PopToRoot();
        UIPanel active{ get; }
    }
}