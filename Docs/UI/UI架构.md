## Unity中的MVC
   - V--对应UGUI或NGUI生成的Unity Object的树形结构，或者其他第三方UI工具生成的类似结构。
   - C--控制类。界面操作的逻辑存放处。
     - 继承MonoBehaviour，挂到View的最上层Object上。
     - 普通类要手动创建，手动管理。
     
## 架构一
    Controller继承MonoBehaviour，并挂到View生成的Prefab上。有一个Manager来管理这些Controller.
    这些Controller直接命名马xxxUI，Manager就叫UIManager。
    
## 架构二
    Controller是普通类，基类就叫Controller.Controller和Controller之间相互调用。实现界面加载和跳转。
    类似ios中的View,Controllerr的关系。不用一个管理类来管理这些Controller.