#region Copyright & License
/*----------------------------------------------------------------
// Copyright (C) 2008 jillzhang 版权所有。 
//  
// 文件名：GifImage.cs
// 文件功能描述：
// 
// 创建标识：jillzhang 
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
//----------------------------------------------------------------*/
/*-------------------------New BSD License ------------------
 Copyright (c) 2008, jillzhang
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

* Neither the name of jillzhang nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Jillzhang.GifUtility
{
    #region 类GifImage - 描述Gif的类
    /// <summary>
    /// 类GifImage - 描述Gif的类
    /// </summary>
    internal class GifImage
    {
        #region 背景图片的长度 
        /// <summary>
        /// 背景图片的长度
        /// </summary>
        internal short Width
        {
            get
            {
                return LogicalScreenDescriptor.Width;
            }         
        }
        #endregion

        #region 背景图片的高度
        /// <summary>
        /// 背景图片的高度
        /// </summary>
        internal short Height
        {
            get
            {
                return LogicalScreenDescriptor.Height;
            }          
        }
        #endregion

        #region gif文件头，可能情况有两种:GIF89a或者GIF87a
        internal string Header { get; set; } = "";
        #endregion

        #region 全局颜色列表
        /// <summary>
        /// 全局颜色列表
        /// </summary>
        internal byte[] GlobalColorTable { get; set; }
        #endregion

        #region Gif的调色板
        /// <summary>
        /// Gif的调色板
        /// </summary>
        internal Color32[] Palette
        {
            get
            {
                Color32[] act = PaletteHelper.GetColor32s(GlobalColorTable);
                act[LogicalScreenDescriptor.BgColorIndex] = new Color32(0);
                return act;
            }
        }
        #endregion

        #region 全局颜色的索引表
        /// <summary>
        /// 全局颜色的索引表
        /// </summary>
        internal Hashtable GlobalColorIndexedTable { get; } = new Hashtable();
        #endregion

        #region 注释扩展块集合
        /// <summary>
        /// 注释块集合
        /// </summary>
        internal List<CommentEx> CommentExtensions { get; set; } = new List<CommentEx>();
        #endregion

        #region 应用程序扩展块集合
        /// <summary>
        /// 应用程序扩展块集合
        /// </summary>
        internal List<ApplicationEx> ApplictionExtensions { get; set; } = new List<ApplicationEx>();
        #endregion

        #region 图形文本扩展集合
        /// <summary>
        /// 图形文本扩展集合
        /// </summary>
        internal List<PlainTextEx> PlainTextEntensions { get; set; } = new List<PlainTextEx>();
        #endregion

        #region 逻辑屏幕描述
        /// <summary>
        /// 逻辑屏幕描述
        /// </summary>
        internal LogicalScreenDescriptor LogicalScreenDescriptor { get; set; }
        #endregion

        #region 解析出来的帧集合
        /// <summary>
        /// 解析出来的帧集合
        /// </summary>
        internal List<GifFrame> Frames { get; set; } = new List<GifFrame>();
        #endregion
    }
    #endregion
}
