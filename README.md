# Donations Offstream
Twip 또는 Toonation 영상후원을 방송에 송출하지 않고 별도의 확장 프로그램으로 공유하는 시스템 (목표)

이것은 Go와 Javascript를 처음 쓰는 자의 피터지는 뚝배기-우선 개발에 관한 이야기


## What-To-do
* 스트리머용 GUI 클라이언트
  + C#/WPF
  + 영상후원 공유기능, 지정한 영상 수동 공유기능

* Pub-Sub 중개서버(필요할 경우)
  + Golang
  + Twitch Extension API를 사용할 경우 중개서버 개발이 필요 없을 것으로 예상됨 

* 시청자용 [트위치 or Chrome] Overlay 확장프로그램
  + HTML/Javascript 예상
  + 위치 이동 및 최소화 가능한 패널, 재생딜레이 조절기능

