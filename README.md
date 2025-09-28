# OpenRA2
An open-source real-time strategy game engine written in C#/.NET8 and developed with Godot (OpenGL). It supports cross-platform operation on Windows and MacOS, has friendly performance and is easy to expand.

一款使用C#/.NET8编写，使用Godot（OpenGL）开发的跨平台即时战略引擎，可以在MacOS和Windows运行。
---
# Modding/Mod扩展
目前我们正在准备Mod开发文档与教程，一般Mod的开发分为两个部分：地图和代码。  
虽然我们允许你使用任何.NET8可以反射加载的.NET动态链接库（dll），但是我们提供的Mod接口是.NET8的，因此如果你使用旧版，可能无法使用Mod框架的接口。  
同时请切记，不要打开c#的AOT编译，否则mod将失去跨平台游玩的功能。  
引擎内有一套开发工具，允许你使用代码来完成触发操作。  
---
# 关于本仓库
本仓库使用MIT协议开源，同时的，这个库中现在还没有什么东西，因为在此之前，我们的这个项目是在Unity中开发的，但由于Unity使用的仍旧是老旧的Mono，无法实现我们的许多功能，又因为我们的精力和学业原因，因此正在将项目从Unity转移到Godot4.5/.NET8中，在短时间内你恐怕无法游玩此游戏。   

若你想要加入这个项目的开发（无偿贡献），可以添加我的QQ号：1752175223

ps：其实这个是针对红色警戒2进行开发的引擎![f0f7b1ae168b0fda40e9a27362c9462c](https://github.com/user-attachments/assets/ac947ae3-907e-4815-8002-fd4fcd0be518)
