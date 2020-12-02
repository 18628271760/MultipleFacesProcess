# MultipleFacesProcess
 Process multiple faces powered by ArcSoft SDK (V3.0)

How to use it£º
0. Make sure the following packages are installed in your windows system
(1) Visual C++ Redistributable Packages for Visual Studio 2013£¨x64£©
(https://www.microsoft.com/en-us/download/details.aspx?id=40784)

(2) .Net 5 £¨x64£©SDK (Yes, it is newest .net5, better than core 3.1)
£¨https://dotnet.microsoft.com/download/visual-studio-sdks£©



1. Register an user account on ArcSoft office web.
(https://ai.arcsoft.com.cn/ucenter/resource/build/index.html#/reg)



2. Create a custom project and download the Windows x64 C++ SDK.



3. Unzip the SDK and copy the 2 dll from lib folder.

4. Download the code from GitHub and open with VS2019. Put the 2 dlls into the ArcSoft library projects. (Choose ¡°always copy¡± to avoid dll-missing mistake)


5. Go back to ArcSoft office web and get your own ¡°APP_ID¡± and ¡°SDK_Key¡±. Copy them to ¡°appsettings.json¡± both in ¡°FaceService¡± and ¡°Video Client¡±



6. Put a folder named ¡°Test¡± in your d Drive(This is hard code in FaceService for test use) and put some face photos(jpg format) inside. And start ¡°FaceService¡± firstly. 
 

Then you can open ¡°Video Client¡± for test.

