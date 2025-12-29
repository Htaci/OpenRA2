# OpenRA2
An open-source real-time strategy game engine written in C# and developed with Unity It supports cross-platform operation on Windows and MacOS, has friendly performance and is easy to expand.

一款使用C#编写，使用Unity开发的跨平台即时战略引擎（未完善，正在更新中），可以在MacOS和Windows运行（未来可能支持部分移动端）。  

区别于OpenRA，OpenRA2的开源协议为Apache-2.0 license协议，你可以自由的使用这个库中的代码，但这并不包含美术资源，因为它属于EA，本项目并不被EA认可，因此使用本项目请遵循EA发布的关于红色警戒2的社区协议，使用本引擎和其Mod必须以免费的形式分发，不得进行商业活动，虽然本项目协议并不限制你的行为，但EA可能会向你追究责任。

关于向下兼容部分的代码，他们是进行了部分改动的，适用于unity，但是对于其他地方的使用不适合，请查看我的另一个项目，这个项目是比较通用的安全实现，在NET8实测可用，内写了注释如何使用。[RA2ResourceKit](https://github.com/Htaci/RA2ResourceKit)

# Modding/Mod扩展
目前我们正在准备Mod开发文档与教程，一般Mod的开发分为两个部分：地图和代码。  
我们允许你使用C#和Xlua编写mod代码。
虽然mod开发大部分，我们提供的编辑器都会帮你处理好各种依赖关系，但同时请切记，不要打开c#的AOT编译，否则mod将失去跨平台游玩的功能。    

# 向下兼容
本项目实现了对Ra2/Ra2YR游戏美术素材的读取，但并不提供保存，如果想要修改地图，将会转为OpenRA2的专用Map地图格式（针对于原版的.map/.yrp）/gif（针对于原版的shp）/json或fbx（针对于原版的vxl体素模型）

# 3D
我们支持了fbx格式的模型导入，支持使用模型动画，同时的，我们将原版的地图素材也改为了3d，因此地图将从原版的2d转变为3d，不过请放心，他的实际效果与原版风格一致，只是因此而获得了更好的3d效果与光影效果，以及很多细节的改进。

# 关于本仓库
本仓库使用Apache-2.0 license协议开源，同时的，这个库中现在的东西还很少，因为我们的精力和学业原因，在短时间内你恐怕无法游玩此游戏。   

若你想要加入这个项目的开发（无偿贡献），可以添加我的QQ号：1752175223，或者加QQ群1033833516（凑热闹也可以来哦）

ps：其实这个是针对红色警戒2进行开发的引擎  
![f0f7b1ae168b0fda40e9a27362c9462c](https://github.com/user-attachments/assets/ac947ae3-907e-4815-8002-fd4fcd0be518)
