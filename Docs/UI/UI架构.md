## 界面管理
### Panel
    - 每个界面就是一个Panel(类似NGUI的Panel)，Panel下正放置其它显示节点。
    - Panel之间通过depth来设置层级关系。
    - Panel是一种特殊的View,由UIManager来管理。
    - Panel继承MonoBehaviour，挂在最上层的Object上。
### UIManager
    - 整个UI的集中管理者
    - 负责Panel的加载、显示、隐藏和删除。
    - 通过Director实现简单的界面跳转功能。
    
## MVC
   - View
     - 对应UGUI或NGUI生成的Unity Object的树形结构(Prefab)，或者其他第三方UI工具生成的类似结构。
     - 没有逻辑功能，通常是Prefab的树形结构或第三方UI生成的Xml或Json的数据格式。
     - 通过RectTransform进行操作。
     - 通过继承UGUI已有的组件来扩展功能
     - 继承UIBehaviour来扩展功能。
     
   - Controller
     - 界面操作的逻辑存放处。
     - 继承MonoBehaviour，挂到View的适当GameObject上。
       - 容易复用Controller,只要挂到GameObject上就行。
       - 和UIManager/Panel可以配合使用。
       - 修改View可能会导致绑定丢失。
     - 不继承MonoBehaviour,不挂到View上。
       - 需要在代码里硬编码和View的关系或通过配置文件来配置和View的关系。
       - 类似Ios中的Controller和View的关系。
       - 布局子Controller的View比较困难。
       - 无法和UIManager/Panel架构完美配合。每个界面都要继承Panel生成一个单独的Panel,然后在初始化的时候创建Controller.
       - 修改View不会影响Controller.
## MVVP
