## 基本概念
### View
    基本构成:
    - 元件(Element):按钮，文本，输入框，图片。
    - 容器(Container):布局器，列表，表格等。
    - 功能快(Widget):HeadBar,BottomMenu
    - 页面(Panel):背包，商店等。

### ViewLogic
    操作View的逻辑。只做和显示相关的操作，如：显示/隐藏，位置，大小，颜色等。
    代码位置:
        - 放在View类里。元件或容器通常放在View类里。功能块看需求。
        - 单独一个类，通常和View是一一对应。这时的View通常指是由UI编辑器生成的,由一些基础View组成，通常没有具体的View类名。
    现有的一些叫法:
        - 狭义的苹果的cocoa中的ViewController(只有操作View的代码)。
        - 狭义的Android的Activity(只有操作View的代码)。
### DataLogic
    数据逻辑。通常业务逻辑除了显示，就是数据的处理。
