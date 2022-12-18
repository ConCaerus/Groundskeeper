using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveData {
    static string saveIndexString = "Save Data Save Index";
    static string saveNameTag = "slotSaveNameTag";
    static string usedTagsTag = "UsedTagsTag";
    static string saveIndexTag() {
        return saveTag(PlayerPrefs.GetInt(saveIndexString));
    }
    static string saveTag(int i) {
        return "Save" + i.ToString() + " ";
    }



    public static void setString(string tag, string save) {
        storeTag(getCurrentSaveIndex(), tag);
        PlayerPrefs.SetString(saveIndexTag() + tag, save);
        PlayerPrefs.Save();
    }
    public static string getString(string tag) {
        return PlayerPrefs.GetString(saveIndexTag() + tag, null);
    }
    public static string getStringInSave(int index, string tag) {
        return PlayerPrefs.GetString(saveTag(index) + tag, null);
    }


    public static void setInt(string tag, int save) {
        storeTag(getCurrentSaveIndex(), tag);
        PlayerPrefs.SetInt(saveIndexTag() + tag, save);
        PlayerPrefs.Save();
    }
    public static int getInt(string tag) {
        return PlayerPrefs.GetInt(saveIndexTag() + tag, 0);
    }
    public static int getIntInSave(int index, string tag) {
        return PlayerPrefs.GetInt(saveTag(index) + tag, 0);
    }


    public static void setFloat(string tag, float save) {
        storeTag(getCurrentSaveIndex(), tag);
        PlayerPrefs.SetFloat(saveIndexTag() + tag, save);
        PlayerPrefs.Save();
    }
    public static float getFloat(string tag) {
        return PlayerPrefs.GetFloat(saveIndexTag() + tag, 0.0f);
    }
    public static float getFloatInSave(int index, string tag) {
        return PlayerPrefs.GetFloat(saveTag(index) + tag, 0.0f);
    }


    public static void deleteKey(string tag) {
        PlayerPrefs.DeleteKey(saveIndexTag() + tag);
        PlayerPrefs.Save();
    }
    public static void deleteKeyInSave(int index, string tag) {
        PlayerPrefs.DeleteKey(saveTag(index) + tag);
        PlayerPrefs.Save();
    }

    public static void deleteCurrentSave() {
        deleteSave(PlayerPrefs.GetInt(saveIndexString));
    }
    public static void deleteSave(int i) {
        var prevIndex = getCurrentSaveIndex();
        setCurrentSaveIndex(i);

        deleteAllTagsInCurrentSave();
        setSaveName(string.Empty);

        setCurrentSaveIndex(prevIndex);
        PlayerPrefs.Save();
    }

    static void deleteAllTagsInCurrentSave() {
        var th = getTagCollector();

        switch(getCurrentSaveIndex()) {
            case 0:
                //  deletes all keys
                foreach(var i in th.t1)
                    deleteKey(i);

                //  saves the new, empty list
                th.t1 = new List<string>();
                saveTagCollector(th);
                break;
            case 1:
                //  deletes all keys
                foreach(var i in th.t1)
                    deleteKey(i);

                //  saves the new, empty list
                th.t1 = new List<string>();
                saveTagCollector(th);
                break;
            case 2:
                //  deletes all keys
                foreach(var i in th.t1)
                    deleteKey(i);

                //  saves the new, empty list
                th.t1 = new List<string>();
                saveTagCollector(th);
                break;
        }
    }


    static void storeTag(int saveIndex, string tag) {
        TagCollector holder = getTagCollector();

        //  adds the new tag into memory if it's new
        switch(saveIndex) {
            case 0:
                if(!holder.t1.Contains(tag))
                    holder.t1.Add(tag);
                break;
            case 1:
                if(!holder.t2.Contains(tag))
                    holder.t2.Add(tag);
                break;
            case 2:
                if(!holder.t3.Contains(tag))
                    holder.t3.Add(tag);
                break;
        }

        //  saves the updated list
        saveTagCollector(holder);
    }
    static TagCollector getTagCollector() {
        TagCollector holder = new TagCollector();
        var data = PlayerPrefs.GetString(usedTagsTag, "");
        if(!string.IsNullOrEmpty(data))
            holder = JsonUtility.FromJson<TagCollector>(data);
        return holder;
    }
    static void saveTagCollector(TagCollector th) {
        var d = JsonUtility.ToJson(th);
        PlayerPrefs.SetString(usedTagsTag, d);
    }

    public static string getSaveName(int i) {
        var prevIndex = getCurrentSaveIndex();
        setCurrentSaveIndex(i);

        var temp = getString(saveNameTag);

        setCurrentSaveIndex(prevIndex);
        return temp;
    }
    public static string getCurrentSaveName() {
        return getString(saveNameTag);
    }
    public static void setSaveName(string name) {
        setString(saveNameTag, name);
        PlayerPrefs.Save();
    }

    public static int getCurrentSaveIndex() {
        return PlayerPrefs.GetInt(saveIndexString);
    }
    public static void setCurrentSaveIndex(int i) {
        PlayerPrefs.SetInt(saveIndexString, i);
    }

    public static bool hasSaveDataForSlot(int i) {
        return getSaveName(i) == string.Empty;
    }
}

//  this stores all of the tags that have been used for each save
//  once a new tag is used that hasn't been used before, it gets stored in here
//  when clearing a save, I just loop over each tag in the desired array
[System.Serializable]
public class TagCollector {
    //  I tried using an array that held these lists, didn't work :(
    public List<string> t1 = new List<string>();
    public List<string> t2 = new List<string>();
    public List<string> t3 = new List<string>();
}
