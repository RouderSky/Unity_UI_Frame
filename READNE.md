# 代码分离Unity UI界面框架
## 核心
- PanelMgr：管理面板的打开、关闭、层级
- PanelBase：所有面板的基类
## 使用方法
- 面板prefab放在Unity项目根目录Resources文件夹下
- 一个面板prefab和一个脚本一一对应
	~~~
	public class <PanelName> : PanelBase {
		//声明控件
		//<PanelName>必须和面板资源中特定控件名字一模一样
		//可以直接通过UI_xxx访问到特定控件
		public GameObject UI_<PanelName>;	

		public override void Init(params object[] args) {
        	base.Init(args);
        	skinPath = <PanelAssetName>;	//该脚本对应的面板prefab在Resource文件夹中的位置
        	layer = PanelLayer.Panel;		//面板的显示层级

			//通过args可以访问到传进来的参数
			Debug.Log((int)args[0])			//->2
			Debug.Log((Bool)args[1])		//->true
    	}	

		public override void OnShowing() {
        	base.OnShowing();

			//设置控件内容

			//设置控件监听
			//实例：为按钮设置监听
        	UI_<PanelName>.GetComponent<Button>().onClick.AddListener(OnStartClick);
    	}

		//监听回调函数
	    void OnStartClick() {
	        Debug.Log("开始游戏");
	    }
	~~~
- 打开特定面板的方法
	~~~
	//第一参数是临时指定的面板prefab路径
	//然后可以跟上任意个数、任意类型的参数
	PanelMgr.instance.OpenPanel<<PanelName>>("",2,true);
	~~~