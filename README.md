# UnityServiceTemplate

## 制作環境

Server: .Net Core 2.0

Client: Unity 2017.4.5LTS(.NET 4.6)

## 動かし方

VisualStudio 2017あたりがあるとよいと思います。

ダウンロードしたプロジェクトの、UnityServiceTemplate.ClientをUnityで開きます。

VisualStudioでUnityServiceTemplate.Clientフォルダ下のUnityServiceTemplate.Client.slnを開きます。

VisualStudioが立ち上がったら、UnityServiceTemplate.Serverを実行します。

サーバが起動したら、UnityでSampleシーンを開いて再生します。

「おはよう」「こんにちは」「こんばんわ」を押して動作を確認します。

## EC2上で動かす

![image](https://user-images.githubusercontent.com/1702680/41492125-f2780458-7137-11e8-91b6-40fe803f2541.png)

インスタンスは「.NET Core with Amazon Linux 2 LTS Candidate - Version 1.0」が便利です。

セキュリティグループのインバウンドにTCPで50051（ないしは指定したやつ）のポートを開けておきます。

インスタンスにPSCPなりで以下のファイルを転送します。

- ビルドしたやつ
  - UnityServiceTemplate.Server.dll
  - UnityServiceTemplate.Server.runtimeconfig.json
- PC内を漁って持っていく
  - Grpc.Core.dll(v1.12.0, .net standard 1.5)
  - System.Interactive.Async.dll(v3.1.1, .net standard 1.3)
  - MessagePack.dll(v1.7.3.4, .net standard 2.0)
  - libgrpc_csharp_ext.x64.so(v1.12.0)
 
次にputtyかなんかでインスタンスに入って、シンボリックリンクを貼ります
 
```ln -s /lib64/libdl-2.26.so /home/ec2-user/libdl.so```
 
ここまできたら、たぶん
 
```dotnet ./UnityServiceTemplate.Server.dll```

でサーバが起動するでしょう。

クライアント側はSampleSceneのlocalhostのあたりに、インスタンスのパブリックDNSを入れればつながると思います。

---

http://www.apache.org/licenses/LICENSE-2.0

Apache License, Version 2.0 のライセンスで配布されている成果物を含んでいます。
