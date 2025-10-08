# OpenRA2
An open-source real-time strategy game engine written in C#/.NET8 and developed with Godot (OpenGL). It supports cross-platform operation on Windows and MacOS, has friendly performance and is easy to expand.

一款使用C#/.NET8编写，使用Godot（OpenGL）开发的跨平台即时战略引擎，可以在MacOS和Windows运行。  

区别于OpenRA，OpenRA2的开源协议为MIT协议，你可以自由的使用这个库中的代码，但这并不包含美术资源，因为它属于EA，本项目并不被EA认可，因此使用本项目请遵循EA发布的关于红色警戒2的社区协议，使用本引擎和其Mod必须以免费的形式分发，不得进行商业活动，虽然本项目协议并不限制你的行为，但EA可能会向你追究责任。

# Modding/Mod扩展
目前我们正在准备Mod开发文档与教程，一般Mod的开发分为两个部分：地图和代码。  
虽然我们允许你使用任何.NET8可以反射加载的.NET动态链接库（dll），但是我们提供的Mod接口是.NET8的，因此如果你使用旧版，可能无法使用Mod框架的接口。  
同时请切记，不要打开c#的AOT编译，否则mod将失去跨平台游玩的功能。  
引擎内有一套开发工具，允许你使用代码来完成触发操作。  

# 单位自定义
区别于原版的单位需要使用ini注册并定义，在本Mod框架下，想要添加新单位并不需要注册，也不需要ini，而是需要继承Mod框架的GameObject类，满足以下条件的，即可通过Mod类库提供的方法直接添加到场景中，或是通过Mod编辑器（可视化）添加到场景中：  
1，该自定义游戏对象类必须继承自GameObject  
2，该自定义游戏对象类必须有一个无参数构造函数  
这种模式对于习惯了制作原版Mod的开发者并不友好，且有一定的上手门槛，但此模式提供的自由度极高，理论上，你可以实现任何你能够写出的逻辑，并且访问游戏的核心逻辑，对ui界面进行改动，添加新的模态窗口等操作。  
与GameObject类似的类还有一个针对于武器弹头的类，这个类在当前版本并未实现，将在后续版本中添加，该类在攻击系统中至关重要，它定义了一个武器的逻辑，以及伤害和针对于护甲的伤害比例，以及武器的攻击样式。  

# 向下兼容
本项目实现了对Ra2/Ra2YR游戏美术素材的读取，但并不提供保存，如果想要修改地图，将会转为OpenRA2的专用Map地图格式（针对于原版的.map/.yrp）/gif（针对于原版的shp）/json或fbx（针对于原版的vxl体素模型）

# 关于本仓库
本仓库使用MIT协议开源，同时的，这个库中现在还没有什么东西，因为在此之前，我们的这个项目是在Unity中开发的，但由于Unity使用的仍旧是老旧的Mono，无法实现我们的许多功能，又因为我们的精力和学业原因，因此正在将项目从Unity转移到Godot4.5/.NET8中，在短时间内你恐怕无法游玩此游戏。   

若你想要加入这个项目的开发（无偿贡献），可以添加我的QQ号：1752175223，或者加QQ群1033833516（凑热闹也可以来哦）

ps：其实这个是针对红色警戒2进行开发的引擎  
![f0f7b1ae168b0fda40e9a27362c9462c](https://github.com/user-attachments/assets/ac947ae3-907e-4815-8002-fd4fcd0be518)
