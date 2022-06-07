using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveData {
    static string saveIndexString = "Save Data Save Index";
    static string saveIndexTag() {
        return saveTag(PlayerPrefs.GetInt(saveIndexString));
    }
    static string saveTag(int i) {
        return "Save" + i.ToString() + " ";
    }


    public static void setString(string tag, string save) {
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

        setCurrentSaveIndex(prevIndex);
        PlayerPrefs.Save();
    }

    public static int getCurrentSaveIndex() {
        return PlayerPrefs.GetInt(saveIndexString);
    }
    public static void setCurrentSaveIndex(int i) {
        PlayerPrefs.SetInt(saveIndexString, i);
    }
}
