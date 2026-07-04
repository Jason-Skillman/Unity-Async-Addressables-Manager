using System;

namespace JasonSkillman.AsyncAddressablesManager
{
	/// <summary>Holds scene names. Used to load/unload scenes in batches.</summary>
	[Serializable]
	public struct SceneBatch
	{
		public string[] scenesToUnload;
		public string[] scenesToLoad;
		public string activeScene;
	}

	/// <summary>Better serializable version of <see cref="SceneBatch"/> for the inspector.</summary>
	[Serializable]
	public struct SceneBatchReference
	{
		public SceneReference.SceneReference[] scenesToUnload;
		public SceneReference.SceneReference[] scenesToLoad;
		public SceneReference.SceneReference activeScene;

		/// <summary>Converts <see cref="SceneBatchReference"/> into <see cref="SceneBatch"/>.</summary>
		public SceneBatch ToSceneContainer()
		{
			SceneBatch s;

			s.scenesToUnload = new string[scenesToUnload.Length];
			for(int i = 0; i < scenesToUnload.Length; i++)
				s.scenesToUnload[i] = scenesToUnload[i].SceneName;

			s.scenesToLoad = new string[scenesToLoad.Length];
			for(int i = 0; i < scenesToLoad.Length; i++)
				s.scenesToLoad[i] = scenesToLoad[i].SceneName;

			s.activeScene = activeScene.SceneName;

			return s;
		}
	}
}
