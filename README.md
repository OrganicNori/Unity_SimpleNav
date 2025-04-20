# Unity_SimpleNav
A Unity scene navigation tool that can quickly store points and switch to the corresponding camera perspectives.
![SimpleNav Screenshot](https://github.com/OrganicNori/Unity_SimpleNav/blob/main/SimpleNav.PNG?raw=true)
SimpleNav is a quick scene navigation tool.
It allows you to record the current camera position of the SceneView and take a screenshot. 
Then, you can quickly switch back to that position. This is very useful for scene editing and design during the white-box stage.
Note: The position information is stored as a scriptable object, along with a PNG screenshot, in the plugin directory.
### Usage
To use, simply copy the SimpleNav directory to any desired location.
In Unity, select Tools > SceneNav Window
- Add NavPoint: This will create a navigation element at the current viewpoint.
- Click the small box icon to switch the viewpoint to the saved location.
- SaveData: This allows you to save the modified Note information and the current list of viewpoints to a file, so that Unity can display this information the next time it starts.
- CleanALL: This will clear all position files and png images.
### Other
If you encounter any issues, you can manually delete the files in the .SceneFile directory.
Additionally, check if the list held by the SceneNav file is cleared. This should generally resolve most problems.


# Unity_SimpleNav

SimpleNav 是一个快速的场景导航工具.可以让你,记录当前SceneView的摄影机位置.

并截图然后快速的切换到该位置.针对于,场景编辑与设计白盒阶段,很有用处.

注:位置信息,存储为scriptableobject,和png截图.到插件目录下.

### 使用方法.
复制SimpleNav目录到任意目录即可使用.

Unity中选择Tools---SceneNav Window

- Add NavPoint---则会在当前的视角位置创建一个导航元素.
  
- 点击,方块小图标,则可以切换视角到保存的位置.
  
- SaveData,可以保存修改的Note信息和当前视角列表到文件中,让Unity下次启动时候也可以显示这些信息.
  
- CleanALL,会清除所有的位置文件.和png图片. 

### 其他.

如果出现问题,可以手动删掉.SceneFile,目录下的文件.

并且,看一下SceneNav,这个文件所Hold的列表是否清空,这样一般就没啥问题了.

