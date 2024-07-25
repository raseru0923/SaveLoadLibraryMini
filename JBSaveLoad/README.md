# ご使用方法

1. 初めにusing JBSaveLoadLib;と記述する。
2. JBSaveLoadから各種関数を利用できます。

### 例

using UnityEngine;
using JBSaveLoadLib; //1

public class Test
{
	CharacterParameter data = new CharacterParameter();
	string filePath = "C:/SaveDatas/PlayerParameter.bin";

	void Start()
	{
		JBSaveLoad.Save(data, filePath); //2のセーブ関数を利用
	}
}


# セーブしたい

1. セーブしたいデータを持つSerializable属性を付与した任意の「class」及び「struct」を用意する。
2. JBSaveLoad.Save(1で用意したデータ,セーブ先のファイルパス)
　※ファイル名は"任意名.bin"で用意してください。
　※存在しないディレクトリ名(フォルダ名)を使用した場合自動的にディレクトリが生成されます。
　※存在しないファイルを使用した場合も自動的にファイルが生成されます。
　※セーブ内容は自動的に暗号化が行われます。

### 例

public class Test
{
	CharacterParameter data = new CharacterParameter();
	string filePath = "C:/SaveDatas/PlayerParameter.bin";

	void Start()
	{
		JBSaveLoad.Save(data, filePath); //2
	}
}

[System.Serializable] //1
public struct CharacterParameter
{
	public float Speed;
	public int MaxHP;
}


# ロードしたい

1. ロードしたいデータと同じ型(「class」及び「struct」)を持った変数を用意する。
2. SaveLoad.Instance.Load(out ロードしたデータを受け取る1, セーブデータの入ったファイルのパス)
　※戻り値はロードの結果です(true=成功false=失敗)。
　※ロードの成功失敗にかかわらず使用前の1のデータは書き換えられます。

### 例

public class Test
{
	CharacterParameter data; //1
	string filePath = "C:/SaveDatas/PlayerParameter.bin";

	void Start()
	{
		if(JBSaveLoad.Load(out data, filePath)/*2*/)
		{
			Debug.Log("成功！");
			return;
		}
		Debug.Log("失敗……");
	}
}


# 注意事項

* バイナリファイル(.bin拡張子のファイル)以外でのセーブについては動作保証できません。
* 暗号化は排他的論理和(XOR)を利用したものであり復号(元データに戻す行為)を完全に防ぐことは保証できません。
* Unityエディター(エディターバージョン2022.3.20f1)以外での動作保証はできません。
* 本ライブラリを利用したファイル操作により起こる問題についての責任は負いかねますので自己責任でお使いください。
* パス指定におけるフォルダやファイルごとの区切りを示す記号は"\"または"/"をお使いください。