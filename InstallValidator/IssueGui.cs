
// 
//     Copyright (C) 2014 CYBUTEK
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace InstallValidator
{
    public class IssueGui : MonoBehaviour
    {
        #region Fields        

        private GUIStyle boxStyle;
        private GUIStyle buttonStyle;
        private bool hasCentred;
        private GUIStyle labelStyle;
        private GUIStyle messageStyle;
        private GUIStyle nameLabelStyle;
        private GUIStyle nameTitleStyle;
        private Rect position = new Rect(Screen.width, Screen.height,0, 0);
        private GUIStyle titleStyle;
        //private bool isInitialised = false;


        #endregion

        #region Methods: protected

        protected void Awake()
        {
            try
            {
                DontDestroyOnLoad(this);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            Log.Info("IssueGui was created.");
        }

        protected void OnGUI()
        {
            try
            {
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, "KSP Installation Validation Monitor", HighLogic.Skin.window);
                this.CentreWindow();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        protected void Start()
        {
            try
            {
                this.InitialiseStyles();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        #endregion

        #region Methods: private

        private void CentreWindow()
        {
            if (this.hasCentred || !(this.position.width > 0) || !(this.position.height > 0))
            {
                return;
            }
            this.position.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            this.hasCentred = true;
        }



        
        private void DrawCompatibilityIssues()
        {
            GUILayout.BeginVertical(this.boxStyle);
            GUILayout.Label("INSTALLATION ISSUES", this.nameTitleStyle);
            foreach (var error in Process.parseErrorMsgs)
            {
                Log.Error("IssueGui, message: " + error);
                GUILayout.Label(error, messageStyle, GUILayout.MinWidth(575.0f));
            }
            GUILayout.EndVertical();
        }



        private void InitialiseStyles()
        {
            //if (Configuration.UseKspSkin)
            //{
            //    GUI.skin = HighLogic.Skin;
            //}
            this.boxStyle = new GUIStyle(HighLogic.Skin.box)
            {
                padding = new RectOffset(10, 10, 5, 5)
            };

            this.nameTitleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                stretchWidth = true
            };

            this.titleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };

            this.nameLabelStyle = new GUIStyle(HighLogic.Skin.label)
            {
                fixedHeight = 25.0f,
                alignment = TextAnchor.MiddleLeft,
                stretchWidth = true
            };

            this.labelStyle = new GUIStyle(HighLogic.Skin.label)
            {
                fixedHeight = 25.0f,
                alignment = TextAnchor.MiddleCenter,
            };

            this.messageStyle = new GUIStyle(HighLogic.Skin.label)
            {
                stretchWidth = true
            };

            this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                }
            };

            //isInitialised = true;
        }

        private void Window(int id)
        {
            try
            {
                if (Process.parseErrorMsgs != null && Process.parseErrorMsgs.Count > 0)
                {
                    this.DrawCompatibilityIssues();
                }
                if (GUILayout.Button("CLOSE", this.buttonStyle))
                {
                    Destroy(this);
                }
                GUI.DragWindow();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        #endregion
    }
}
