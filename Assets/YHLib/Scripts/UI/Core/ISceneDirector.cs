namespace YH.UI
{
    public interface ISceneDirector
    {
        void RunWithScene(string sceneName);

        void PushScene(string sceneName);

        void PopScene();

        void ReplaceScene(string sceneName);

        void PopToSceneStackLevel(int level);

        void PopToRootScene();
    }
}