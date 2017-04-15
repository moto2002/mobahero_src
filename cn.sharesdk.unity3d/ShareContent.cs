using System;
using System.Collections;
using UnityEngine;

namespace cn.sharesdk.unity3d
{
	public class ShareContent
	{
		private Hashtable shareParams = new Hashtable();

		private Hashtable customizeShareParams = new Hashtable();

		public void SetTitle(string title)
		{
			this.shareParams["title"] = title;
		}

		public void SetText(string text)
		{
			this.shareParams["text"] = text;
		}

		public void SetUrl(string url)
		{
			this.shareParams["url"] = url;
		}

		public void SetImagePath(string imagePath)
		{
			this.shareParams["imagePath"] = imagePath;
		}

		public void SetImageUrl(string imageUrl)
		{
			this.shareParams["imageUrl"] = imageUrl;
		}

		public void SetShareType(int shareType)
		{
			if (shareType == 0)
			{
				shareType = 1;
			}
			this.shareParams["shareType"] = shareType;
		}

		public void SetTitleUrl(string titleUrl)
		{
			this.shareParams["titleUrl"] = titleUrl;
		}

		public void SetComment(string comment)
		{
			this.shareParams["comment"] = comment;
		}

		public void SetSite(string site)
		{
			this.shareParams["site"] = site;
		}

		public void SetSiteUrl(string siteUrl)
		{
			this.shareParams["siteUrl"] = siteUrl;
		}

		public void SetAddress(string address)
		{
			this.shareParams["address"] = address;
		}

		public void SetFilePath(string filePath)
		{
			this.shareParams["filePath"] = filePath;
		}

		public void SetMusicUrl(string musicUrl)
		{
			this.shareParams["musicUrl"] = musicUrl;
		}

		public void SetLatitude(string latitude)
		{
			this.shareParams["latitude"] = latitude;
		}

		public void SetLongitude(string longitude)
		{
			this.shareParams["longitude"] = longitude;
		}

		public void SetSource(string source)
		{
			this.shareParams["url"] = source;
		}

		public void SetAuthor(string author)
		{
			this.shareParams["address"] = author;
		}

		public void SetSafetyLevel(int safetyLevel)
		{
			this.shareParams["safetyLevel"] = safetyLevel;
		}

		public void SetContentType(int contentType)
		{
			this.shareParams["contentType"] = contentType;
		}

		public void SetHidden(int hidden)
		{
			this.shareParams["hidden"] = hidden;
		}

		public void SetIsPublic(bool isPublic)
		{
			this.shareParams["isPublic"] = isPublic;
		}

		public void SetIsFriend(bool isFriend)
		{
			this.shareParams["isFriend"] = isFriend;
		}

		public void SetIsFamily(bool isFamily)
		{
			this.shareParams["isFamily"] = isFamily;
		}

		public void SetFriendsOnly(bool friendsOnly)
		{
			this.shareParams["isFriend"] = friendsOnly;
		}

		public void SetGroupID(string groupID)
		{
			this.shareParams["groupID"] = groupID;
		}

		public void SetAudioPath(string audioPath)
		{
			this.shareParams["filePath"] = audioPath;
		}

		public void SetVideoPath(string videoPath)
		{
			this.shareParams["filePath"] = videoPath;
		}

		public void SetNotebook(string notebook)
		{
			this.shareParams["notebook"] = notebook;
		}

		public void SetTags(string tags)
		{
			this.shareParams["tags"] = tags;
		}

		public void SetObjectID(string objectId)
		{
			this.shareParams["objectID"] = objectId;
		}

		public void SetAlbumID(string albumId)
		{
			this.shareParams["AlbumID"] = albumId;
		}

		public void SetEmotionPath(string emotionPath)
		{
			this.shareParams["emotionPath"] = emotionPath;
		}

		public void SetExtInfoPath(string extInfoPath)
		{
			this.shareParams["extInfoPath"] = extInfoPath;
		}

		public void SetSourceFileExtension(string sourceFileExtension)
		{
			this.shareParams["sourceFileExtension"] = sourceFileExtension;
		}

		public void SetSourceFilePath(string sourceFilePath)
		{
			this.shareParams["sourceFilePath"] = sourceFilePath;
		}

		public void SetThumbImageUrl(string thumbImageUrl)
		{
			this.shareParams["thumbImageUrl"] = thumbImageUrl;
		}

		public void SetUrlDescription(string urlDescription)
		{
			this.shareParams["urlDescription"] = urlDescription;
		}

		public void SetBoard(string SetBoard)
		{
			this.shareParams["board"] = SetBoard;
		}

		public void SetMenuX(float menuX)
		{
			this.shareParams["menuX"] = menuX;
		}

		public void SetMenuY(float menuY)
		{
			this.shareParams["menuY"] = menuY;
		}

		public void SetVisibility(string visibility)
		{
			this.shareParams["visibility"] = visibility;
		}

		public void SetBlogName(string blogName)
		{
			this.shareParams["blogName"] = blogName;
		}

		public void SetRecipients(string recipients)
		{
			this.shareParams["recipients"] = recipients;
		}

		public void SetCCRecipients(string ccRecipients)
		{
			this.shareParams["ccRecipients"] = ccRecipients;
		}

		public void SetBCCRecipients(string bccRecipients)
		{
			this.shareParams["bccRecipients"] = bccRecipients;
		}

		public void SetAttachmentPath(string attachmentPath)
		{
			this.shareParams["attachmentPath"] = attachmentPath;
		}

		public void SetDesc(string desc)
		{
			this.shareParams["desc"] = desc;
		}

		public void SetIsPrivateFromSource(bool isPrivateFromSource)
		{
			this.shareParams["isPrivateFromSource"] = isPrivateFromSource;
		}

		public void SetResolveFinalUrl(bool resolveFinalUrl)
		{
			this.shareParams["resolveFinalUrl"] = resolveFinalUrl;
		}

		public void SetFolderId(int folderId)
		{
			this.shareParams["folderId"] = folderId;
		}

		public void SetTweetID(string tweetID)
		{
			this.shareParams["tweetID"] = tweetID;
		}

		public void SetToUserID(string toUserID)
		{
			this.shareParams["toUserID"] = toUserID;
		}

		public void SetPermission(string permission)
		{
			this.shareParams["permission"] = permission;
		}

		public void SetEnableShare(bool enableShare)
		{
			this.shareParams["enableShare"] = enableShare;
		}

		public void SetImageWidth(float imageWidth)
		{
			this.shareParams["imageWidth"] = imageWidth;
		}

		public void SetImageHeight(float imageHeight)
		{
			this.shareParams["imageHeight"] = imageHeight;
		}

		public void SetAppButtonTitle(string appButtonTitle)
		{
			this.shareParams["appButtonTitle"] = appButtonTitle;
		}

		public void SetAndroidExecParam(Hashtable androidExecParam)
		{
			this.shareParams["androidExecParam"] = androidExecParam;
		}

		public void SetAndroidMarkParam(string androidMarkParam)
		{
			this.shareParams["androidMarkParam"] = androidMarkParam;
		}

		public void SetIphoneExecParam(Hashtable iphoneExecParam)
		{
			this.shareParams["iphoneExecParam"] = iphoneExecParam;
		}

		public void SetIphoneMarkParam(string iphoneMarkParam)
		{
			this.shareParams["iphoneMarkParam"] = iphoneMarkParam;
		}

		public void SetIpadExecParam(Hashtable ipadExecParam)
		{
			this.shareParams["ipadExecParam"] = ipadExecParam;
		}

		public void SetIpadMarkParam(string ipadMarkParam)
		{
			this.shareParams["ipadMarkParam"] = ipadMarkParam;
		}

		public void SetShareContentCustomize(PlatformType platform, ShareContent content)
		{
			this.customizeShareParams[(int)platform] = content.GetShareParamsStr();
		}

		public string GetShareParamsStr()
		{
			if (this.customizeShareParams.Count > 0)
			{
				this.shareParams["customizeShareParams"] = this.customizeShareParams;
			}
			string text = MiniJSON.jsonEncode(this.shareParams);
			Debug.Log("ParseShareParams  ===>>> " + text);
			return text;
		}

		public Hashtable GetShareParams()
		{
			if (this.customizeShareParams.Count > 0)
			{
				this.shareParams["customizeShareParams"] = this.customizeShareParams;
			}
			string str = MiniJSON.jsonEncode(this.shareParams);
			Debug.Log("ParseShareParams  ===>>> " + str);
			return this.shareParams;
		}
	}
}
