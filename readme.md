# Roid1 URDF を VMCプロトコル で動かすもの

## 開発環境
- Unity 2020.3.20


## 含まれるシーン
Scenes フォルダ内に下記があります。
分かりやすい順は以下です。

- SimpleAnimation : AnimatorController で指定されたモーションを繰り返します。速度パラメータ違いのロボットを並べてあります。
- VMCProtocolReceiver : VMCプロトコルを受信して動きます。速度パラメータ違いのロボットを並べてあります。
- DesktopMascot : ウィンドウを透過してデスクトップマスコット風に表示する例です。


## 使用ライブラリ

### このリポジトリに含まれるもの
| Name | Author | Description |
|:-----|:-------|:------------|
| [Roid1 URDF](https://github.com/Ninagawa123/roid1_urdf) | @Ninagawa123 さん | ロボットのモデル |
| [EVMC4U](https://github.com/gpsnmeajp/EasyVirtualMotionCaptureForUnity) | @gpsnmeajp さん | VMCプロトコル受信 |
| [Running Motion～色んな走りモーション～](https://booth.pm/ja/items/2845548) | 梅干し大好きっ子クラブさん | モーション例 |

### Unityで開く際に Package Manager によって自動ダウンロードされるもの
| Name | Author | Description |
|:-----|:-------|:------------|
| [URDF Importer](https://github.com/Unity-Technologies/URDF-Importer) | Unity Technologies | URDF利用に必要 |
| [UniVRM](https://github.com/vrm-c/UniVRM) | VRM Consortium | VRMモデル利用に必要 |
| [UniGLTF](https://github.com/vrm-c/UniVRM/tree/master/Assets/UniGLTF) | ousttrue | UniVRMとセット |
| [VRM Shaders](https://github.com/vrm-c/UniVRM/tree/master/Assets/VRMShaders) | Masataka SUMI for MToon | UniVRMとセット |
| [VRM-1.0β](https://github.com/vrm-c/UniVRM/tree/master/Assets/VRM10) | VRM Consortium | UniVRMとセット |
| [UniWindowController](https://github.com/kirurobo/UniWindowController) | @kirurobo | ウィンドウ透過 |
