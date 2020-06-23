#region Copyright & License
/*----------------------------------------------------------------
// Copyright (C) 2008 jillzhang ��Ȩ���С� 
//  
// �ļ�����GifImage.cs
// �ļ�����������
// 
// ������ʶ��jillzhang 
// �޸ı�ʶ��
// �޸�������
//
// �޸ı�ʶ��
// �޸�������
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
    #region ��GifImage - ����Gif����
    /// <summary>
    /// ��GifImage - ����Gif����
    /// </summary>
    internal class GifImage
    {
        #region ����ͼƬ�ĳ��� 
        /// <summary>
        /// ����ͼƬ�ĳ���
        /// </summary>
        internal short Width
        {
            get
            {
                return LogicalScreenDescriptor.Width;
            }         
        }
        #endregion

        #region ����ͼƬ�ĸ߶�
        /// <summary>
        /// ����ͼƬ�ĸ߶�
        /// </summary>
        internal short Height
        {
            get
            {
                return LogicalScreenDescriptor.Height;
            }          
        }
        #endregion

        #region gif�ļ�ͷ���������������:GIF89a����GIF87a
        internal string Header { get; set; } = "";
        #endregion

        #region ȫ����ɫ�б�
        /// <summary>
        /// ȫ����ɫ�б�
        /// </summary>
        internal byte[] GlobalColorTable { get; set; }
        #endregion

        #region Gif�ĵ�ɫ��
        /// <summary>
        /// Gif�ĵ�ɫ��
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

        #region ȫ����ɫ��������
        /// <summary>
        /// ȫ����ɫ��������
        /// </summary>
        internal Hashtable GlobalColorIndexedTable { get; } = new Hashtable();
        #endregion

        #region ע����չ�鼯��
        /// <summary>
        /// ע�Ϳ鼯��
        /// </summary>
        internal List<CommentEx> CommentExtensions { get; set; } = new List<CommentEx>();
        #endregion

        #region Ӧ�ó�����չ�鼯��
        /// <summary>
        /// Ӧ�ó�����չ�鼯��
        /// </summary>
        internal List<ApplicationEx> ApplictionExtensions { get; set; } = new List<ApplicationEx>();
        #endregion

        #region ͼ���ı���չ����
        /// <summary>
        /// ͼ���ı���չ����
        /// </summary>
        internal List<PlainTextEx> PlainTextEntensions { get; set; } = new List<PlainTextEx>();
        #endregion

        #region �߼���Ļ����
        /// <summary>
        /// �߼���Ļ����
        /// </summary>
        internal LogicalScreenDescriptor LogicalScreenDescriptor { get; set; }
        #endregion

        #region ����������֡����
        /// <summary>
        /// ����������֡����
        /// </summary>
        internal List<GifFrame> Frames { get; set; } = new List<GifFrame>();
        #endregion
    }
    #endregion
}
