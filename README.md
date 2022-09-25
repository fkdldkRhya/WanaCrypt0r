WanaCrypt0r
===
![WanaCrypt0r Main](main-image.png)

### WanaCrypt0r (RHYA.Network Version)
실제 `WannaCry`와 `WanaCrypt0r` 랜섬웨어를 참고하여 만든 실험용 랜섬웨어입니다.

### WanaCrypt0r 작동 방식
`RHYA.Network` 서버에 접속하여 Kill-Switch를 확인하고 스위치가 꺼져 있다면 `d.wnry`파일을 내려받고 해당 파일의 압축을 해제합니다. 압축 해제가 완료되면 `tasksche.exe`를 실행하고 해당 프로그램의 모든 작업을 끝맞추었다면 `@WanaDecryptor@.exe`와 `taskdl.exe` 실행하고 프로그램을 종료합니다.

### 더 자세한 설명
> a.wnry (암호화 되어있지 않음) 
>> 암호화 대상 폴더 경로 저장 
>> 
> b.wnry (암호화 되어있음) 
>> 클라이언트 ID 및 파일 암호화 키 
>> _(해당 데이터를 파일에 쓰기전에 서버에 전송하여 서버가 자신이 가지고 있는 AES키를 통해 암호화 해서 받은 내용을 쓰는 것이므로 클라이언트에서 복호화는 불가능 함.)_ 
>> 
> c.wnry 
>> 비트코인 지갑 주소 정보 파일 
>> 
> d.wnry 
>> WanaCrypt0r 파일을 담고있는 압축파일 (ZIP 파일 암호: wncry@YhccKSJFE2JhkJJ) 
>> 
> e.wnry 
>> @WanaDecrypt0r@.exe 배경화면 이미지 파일 
>> 
> p.wnry (암호화 되어있지 않음) 
>> 무료 복호화 파일 정보 / 복호화 진행 여부 
>> 
> 0000000.bin (암호화 되어있지 않음) 
>> 무료 복호화 파일 정보 / 파일 리스트 
>> 
> 0000000.xkey (암호화 되어있지 않음) 
>> 무료 복호화 파일 정보 / 파일 암호화 키 
>> 
> taskdl.exe
>> njRAT 7.0d 로 빌드한 Backdoor
>> Host: `rhya.ddns.net`
>> VicTim Name: `RHYA.Network`
>> Copy: True
>> ExeName: WindowsServices.exe
>> Directory: %TEMP%
>> Protect Process (BSOD): False
>> Copy To StartUp: True
>> Registy StarUp: True
>> Anti's: True
>> Spread: True
>> Obfuscation: False
