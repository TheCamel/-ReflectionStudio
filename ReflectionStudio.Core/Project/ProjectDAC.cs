using System;
using System.IO;
using ReflectionStudio.Core.FileManagement;
using System.Collections.Generic;
using ReflectionStudio.Core.Events;

namespace ReflectionStudio.Core.Project
{
	class ProjectDAC
	{
		/// <summary>
		/// Load the project in the Current property
		/// </summary>
		/// <returns></returns>
		public ProjectEntity Load( string filePath )
		{
			ProjectEntity entity = null;
			try
			{
				if (File.Exists(filePath))
				{
					entity = (ProjectEntity)SerializeHelper.Deserialize(filePath, typeof(ProjectEntity));

					//first check the solution folder
					CheckProjectDir(entity);
				}
			}
			catch (Exception error)
			{
				Tracer.Error("ProjectDAC.Load", error);
			}
			return entity;
		}

		/// <summary>
		/// Save the current project
		/// </summary>
		/// <returns></returns>
		public bool Save( ProjectEntity entity )
		{
			//first check the solution folder
			CheckProjectDir( entity );

			if (SerializeHelper.Serialize(entity.ProjectFilePath, entity))
			{
				entity.IsChanged = entity.Settings.IsChanged = false;
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Create a new project
		/// </summary>
		/// <returns></returns>
		public ProjectEntity Create(string projectLocation, string projectName)
		{
			ProjectEntity entity = null;
			try
			{
				entity = new ProjectEntity(projectLocation, projectName);

				if (File.Exists(entity.ProjectFilePath))
					return null;

				//first check the solution folder
				CheckProjectDir(entity);
			}
			catch (Exception error)
			{
				Tracer.Error("ProjectDAC.Create", error);
			}
			return entity;
		}

		private void CheckProjectDir(ProjectEntity entity)
		{
			//first create the solution folder
			if (!Directory.Exists(entity.ProjectPath))
				Directory.CreateDirectory(entity.ProjectPath);

			//create sub folders
			if (!Directory.Exists(entity.SubFolderProfiled))
				Directory.CreateDirectory(entity.SubFolderProfiled);

			if (!Directory.Exists(entity.SubFolderSnapshot))
				Directory.CreateDirectory(entity.SubFolderSnapshot);
		}
	}
}
