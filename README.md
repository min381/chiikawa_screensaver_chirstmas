Windows에서 실행하는 방법
1. 위 ZIP 파일을 Windows PC에 압축 해제해.
2. .NET 8 SDK를 설치해.
3. 프로젝트 폴더에서 PowerShell을 열고 다음 명령을 실행해.
dotnet publish -c Release -r win-x64 --self-contained true


4. 빌드 결과물인 ChiikawaScreensaver.exe를 ChiikawaScreensaver.scr로 이름을 바꿔.
5. Assets 폴더가 실행 파일과 함께 있는지 확인하고 실행해.
