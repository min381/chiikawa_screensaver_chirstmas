# 치이카와 Windows 스크린세이버 프로젝트

## 포함 파일
- `Assets/`: 포스터에서 분리한 캐릭터 8개 PNG (투명 배경)
- `Program.cs`: WinForms 스크린세이버 구현
- `ChiikawaScreensaver.csproj`: .NET 프로젝트
- `app.manifest`: Windows 실행 매니페스트

## 동작
- 시작 즉시 캐릭터 1마리 등장
- 이후 10~30초 사이 무작위 간격으로 새 캐릭터 추가
- 캐릭터는 좌우로 걷듯 이동하며 통통 튀고 몸이 좌우로 기울어짐
- 최대 8마리까지 동시에 표시
- 키 입력, 마우스 클릭 또는 일정 거리 이상의 마우스 이동으로 종료

## 빌드 방법 (Windows)
1. Windows PC에 **.NET 8 SDK** 설치: https://dotnet.microsoft.com/download/dotnet/8.0
2. 이 폴더를 Windows PC에 압축 해제합니다.
3. PowerShell 또는 명령 프롬프트에서 이 폴더로 이동합니다.
4. 다음 명령을 실행합니다.

```powershell
dotnet publish -c Release -r win-x64 --self-contained true
```

5. 결과 실행 파일을 다음 경로에서 찾습니다.

`bin\Release\net8.0-windows\win-x64\publish\ChiikawaScreensaver.exe`

6. 파일 이름을 `ChiikawaScreensaver.scr`로 바꾸고, `Assets` 폴더도 같은 디렉터리에 둡니다. (publish 결과에 Assets가 복사됐는지 확인)
7. `.scr` 파일을 더블클릭해 테스트하거나, Windows의 `C:\Windows\System32`에 복사한 뒤 바탕화면 → 개인 설정 → 화면 보호기에서 선택합니다. 시스템 폴더에 복사하려면 관리자 권한이 필요할 수 있습니다.

## 테스트
개발 중에는 `dotnet run`으로 실행할 수 있습니다. 화면 보호기를 종료하려면 키를 누르거나 클릭하거나 마우스를 움직이세요.

## 애니메이션 관련 참고
원본 포스터의 캐릭터는 각자 한 장의 정지 그림입니다. 현재 프로젝트는 원본의 외형을 유지하면서 이동, 상하 바운스, 좌우 기울임으로 걷는 느낌을 냅니다. 다리 관절이 실제로 교대로 움직이는 프레임 애니메이션은 원본에 없는 관절/팔다리 정보를 새로 그려야 하므로 이 버전에는 포함하지 않았습니다.
