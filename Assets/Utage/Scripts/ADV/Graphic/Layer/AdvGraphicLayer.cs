// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UtageExtensions;
namespace Utage
{

	/// <summary>
	/// グラフィックのレイヤー管理の基底クラス
	/// </summary>
	public abstract class AdvGraphicLayer : MonoBehaviour, IAdvGraphicLayer
	{
		public AdvLayerSettingData SettingData { get; protected set; }
		abstract internal AdvLayerSettingData.LayerType LayerType { get; }

		public AdvEngine Engine { get { return Manager.Engine; } }

		public AdvGraphicManager Manager { get; protected set; }

		protected Transform RootObjects { get; set; }
		public AdvGraphicObject DefaultObject { get; protected set; }
		public Dictionary<string, AdvGraphicObject> CurrentGraphics
		{
			get { return currentGraphics; }
		}
		Dictionary<string, AdvGraphicObject> currentGraphics = new Dictionary<string, AdvGraphicObject>();

		public Canvas Canvas { get; protected set; }

		abstract internal void Init(AdvGraphicManager manager);

		//　キャンバスのRectTransformをリセットして初期状態に
		abstract internal void ResetCanvasRectTransform();


		internal void Remove(AdvGraphicObject obj)
		{
			if (CurrentGraphics.ContainsValue(obj))
			{
				CurrentGraphics.Remove(obj.name);
			}
			if (DefaultObject == obj)
			{
				DefaultObject = null;
			}
		}

		//オブジェクトを描画する
		internal AdvGraphicObject Draw(string name, AdvGraphicOperaitonArg arg )
		{
			AdvGraphicObject obj = GetObjectCreateIfMissing(name, arg.Graphic);
			obj.Loader.LoadGraphic(arg.Graphic, () =>
			{
				obj.Draw(arg, arg.GetSkippedFadeTime(Engine));
			});
			return obj;
		}

		//デフォルトオブジェクトとして描画する
		internal AdvGraphicObject DrawToDefault(string name, AdvGraphicOperaitonArg arg)
		{
			bool changeObject = false;
			bool keepPosition = false;
			Vector3 oldPosition = Vector3.zero;

			if (DefaultObject != null && DefaultObject.LastResource != null)
			{
				if (DefaultObject.name != name)
				{
					//デフォルトオブジェクトの名前が違うなら、そのオブジェクトは変更
					//場所も保持しない
					changeObject = true;
				}
				else
				{
					if (CheckFailedCrossFade(arg))
					{
						//クロスフェードに失敗するだけの場合
						//場所は保持する
						changeObject = true;
						keepPosition = true;
						oldPosition = DefaultObject.transform.localPosition;
					}
					else
					{
						//クロスフェードできるならオブジェクトの変更を行わない
						changeObject = false;
					}
				}
			}
			if (changeObject)
			{
				//フェードアウトする
				if (LayerType == AdvLayerSettingData.LayerType.Bg)
				{
					DelayOut(DefaultObject.name, arg.GetSkippedFadeTime(Engine));
				}
				else
				{
					FadeOut(DefaultObject.name, arg.GetSkippedFadeTime(Engine));
				}
			}
			DefaultObject = Draw(name,arg);

			//前の場所を保持する
			if (keepPosition && !Manager.IgnoreKeepPositionOnCrossFade)
			{
				DefaultObject.transform.localPosition = oldPosition;
			}
			return DefaultObject;
		}

		protected virtual bool CheckFailedCrossFade(AdvGraphicOperaitonArg arg)
		{
			if (arg.Graphic.FileType != DefaultObject.LastResource.FileType) return true;
			return DefaultObject.TargetObject.CheckFailedCrossFade(arg.Graphic);
		}

		//指定の名前のオブジェクトを取得、なければ作成
		internal AdvGraphicObject GetObjectCreateIfMissing(string name, AdvGraphicInfo grapic)
		{
			if (grapic == null) 
			{
				Debug.LogError ( name + " grapic is null");
				return null;
			}
			AdvGraphicObject obj;
			if (!currentGraphics.TryGetValue(name, out obj))
			{
				//まだ作成されてないから作る
				obj = CreateObject(name, grapic);
			}
			return obj;
		}

		//描画オブジェクトを作成
		protected virtual AdvGraphicObject CreateObject(string name, AdvGraphicInfo grapic, bool resetOnFirst = true)
		{
			AdvGraphicObject obj;
			//IAdvGraphicObjectがAddComponentされたプレハブをリソースに持つかチェック
			GameObject prefab;
			if (grapic.TryGetAdvGraphicObjectPrefab(out prefab))
			{
				//プレハブからリソースオブジェクトを作成して返す
				GameObject go = GameObject.Instantiate(prefab);
				go.name = name;
				obj = go.GetComponent<AdvGraphicObject>();
				RootObjects.AddChild(obj.gameObject);
			}
			else
			{
				obj = RootObjects.AddChildGameObjectComponent<AdvGraphicObject>(name);
			}
			obj.Init(this, grapic);

			//最初の描画時は位置をリセットする
			if (resetOnFirst && currentGraphics.Count == 0)
			{
				this.ResetCanvasRectTransform();
			}

			currentGraphics.Add(obj.name, obj);
			return obj;
		}

		//フェードアウト
		internal void FadeOut(string name, float fadeTime)
		{
			AdvGraphicObject obj;
			if (currentGraphics.TryGetValue(name, out obj))
			{
				obj.FadeOut(fadeTime);
				Remove(obj);
			}
		}

		//一定時間後にフェードなしで消える
		internal void DelayOut(string name, float delay)
		{
			AdvGraphicObject obj;
			if (currentGraphics.TryGetValue(name, out obj))
			{
				Remove(obj);
				StartCoroutine(CoDelayOut(obj,delay));
			}
		}

		IEnumerator CoDelayOut(AdvGraphicObject obj, float delay)
		{
			yield return Engine.Time.WaitForSeconds(delay);
			if(obj!=null) obj.Clear();
		}


		internal void FadeOutAll(float fadeTime)
		{
			List<AdvGraphicObject> values = new List<AdvGraphicObject>(currentGraphics.Values);
			foreach (var obj in values)
			{
				obj.FadeOut(fadeTime);
			}
			currentGraphics.Clear();
			DefaultObject = null;
		}

		//指定名のパーティクルを非表示にする
		internal void FadeOutParticle(string name)
		{
			AdvGraphicObject obj;
			if (currentGraphics.TryGetValue(name, out obj))
			{
				if (obj.TargetObject is AdvGraphicObjectParticle)
				{
					obj.FadeOut(0);
					Remove(obj);
				}
			}
		}

		//パーティクルを全て非表示にする
		internal void FadeOutAllParticle()
		{
			List<AdvGraphicObject> values = new List<AdvGraphicObject>(currentGraphics.Values);
			foreach (var obj in values)
			{
				if (obj.TargetObject is AdvGraphicObjectParticle)
				{
					obj.FadeOut(0);
					Remove(obj);
				}
			}
		}

		//クリア処理
		internal void Clear()
		{
			List<AdvGraphicObject> values = new List<AdvGraphicObject>(currentGraphics.Values);
			foreach (var obj in values)
			{
				obj.Clear();
			}
			currentGraphics.Clear();
			DefaultObject = null;
		}

		//デフォルトグラフィックオブジェクトの名前が指定名と同じかチェック
		internal bool IsEqualDefaultGraphicName(string name)
		{
			if (DefaultObject!=null)
			{
				return DefaultObject.name == name;
			}
			return false;
		}

		//指定名のオブジェクトがあるか
		internal bool Contains(string name)
		{
			return currentGraphics.ContainsKey(name);
		}

		//指定名のオブジェクトがあれば返す
		internal AdvGraphicObject Find(string name)
		{
			AdvGraphicObject obj;
			if(currentGraphics.TryGetValue(name,out obj))
			{
				return obj;
			}
			return null;
		}


		internal void AddAllGraphics(List<AdvGraphicObject> graphics)
		{
			graphics.AddRange(currentGraphics.Values);
		}

		//ロード中かチェック
		internal bool IsLoading
		{
			get
			{
				foreach (var keyValue in currentGraphics)
				{
					if (keyValue.Value.Loader.IsLoading) return true;
				}
				return false;
			}
		}

		const int Version = 0;
		//セーブデータ用のバイナリ書き込み
		public virtual void Write(BinaryWriter writer)
		{
			writer.Write(Version);
			writer.WriteLocalTransform(this.transform);

			int count = 0;
			foreach (var keyValue in CurrentGraphics)
			{
				if (keyValue.Value.LastResource.DataType == AdvGraphicInfo.TypeCapture)
				{
					Debug.LogError("Caputure image not support on save");
					continue;
				}
				++count;
			}

			writer.Write(count);
			foreach (var keyValue in CurrentGraphics)
			{
				if (keyValue.Value.LastResource.DataType == AdvGraphicInfo.TypeCapture)
				{
					continue;
				}

				writer.Write(keyValue.Key);
				writer.WriteBuffer(keyValue.Value.LastResource.OnWrite);
				writer.WriteBuffer(keyValue.Value.Write);
			}
			writer.Write(DefaultObject == null ? "" : DefaultObject.name);
		}

		//セーブデータ用のバイナリ読み込み
		public virtual void Read(BinaryReader reader)
		{
			int version = reader.ReadInt32();
			if (version < 0 || version > Version)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, version));
				return;
			}

			reader.ReadLocalTransform(this.transform);

			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				string key = reader.ReadString();
				AdvGraphicInfo graphic = null;
				reader.ReadBuffer(x => graphic = AdvGraphicInfo.ReadGraphicInfo(Engine, x));
				byte[] buffer = reader.ReadBuffer();
				AdvGraphicObject obj = CreateObject(key, graphic,false);
				obj.Read(buffer, graphic);
			}
			string defaulObjectName = reader.ReadString();
			DefaultObject = Find(defaulObjectName);
		}
	}
}
