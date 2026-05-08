using UnityEngine;
using System.Collections;

namespace XD.Prefs {
    [System.Serializable]
    public class AdvancedPPfs {
        //setup core
        public static readonly AdvancedPPfs Core = new AdvancedPPfs();

        public static event System.Action OnDataReset;

        ProtectedIntMemory[] BurnIntMemory = new ProtectedIntMemory[0];
        ProtectedFloatMemory[] BurnFloatMemory = new ProtectedFloatMemory[0];

        public void ProtectedStore(string key, int value) {
            PlayerPrefs.SetInt(key, value);
            Protect(key, value);
            Debug.Log($"Resived Protected-Store Input(int), including : {key} = {value}");
        }
        public void ProtectedStore(string key, float value) {
            PlayerPrefs.SetFloat(key, value);
            Protect(key, value);
            Debug.Log($"Resived Protected-Store Input(float), including : {key} = {value}");
        }
        void Protect(string key, int value) {
            if (FindProtectiveInt(key) != -1) {
                ProtectedIntMemory burn = BurnIntMemory[FindProtectiveInt(key)];
                if (burn != null) {
                    burn.key = key;
                    burn.value = value;
                }
            } else { 
                var product = new ProtectedIntMemory();
                product.key = key;
                product.value = value;
                //Extend the array that way...
                ProtectedIntMemory[] temp = new ProtectedIntMemory[BurnIntMemory.Length + 1];
                System.Array.Copy(BurnIntMemory, temp, BurnIntMemory.Length);
                temp[temp.Length - 1] = product;

                BurnIntMemory = temp;
            }
            //foreach(ProtectedIntMemory prot in BurnIntMemory) { "Debug.Log($"{prot.key} = {prot.value}"); }
        }
        void Protect(string key, float value) {
            if (FindProtectiveInt(key) != -1) {
                ProtectedFloatMemory burn = BurnFloatMemory[FindProtectiveFloat(key)];
                if (burn != null) {
                    burn.key = key;
                    burn.value = value;
                }
            } else { 
                var product = new ProtectedFloatMemory();
                product.key = key;
                product.value = value;
                //Extend the array that way...
                ProtectedFloatMemory[] temp = new ProtectedFloatMemory[BurnFloatMemory.Length + 1];
                System.Array.Copy(BurnFloatMemory, temp, BurnFloatMemory.Length);
                temp[temp.Length - 1] = product;

                BurnFloatMemory = temp;
            }
            //foreach(ProtectedFloatMemory prot in BurnIntMemory) { "Debug.Log($"{prot.key} = {prot.value}"); }
        }
        int FindProtectiveInt(string key) {
            for (int i = 0; i < BurnIntMemory.Length; i++) {
                if (BurnIntMemory[i].key == key) {
                    return i;
                }
            } 
            return -1;
        }
        int FindProtectiveFloat(string key) {
            for (int i = 0; i < BurnFloatMemory.Length; i++) {
                if (BurnFloatMemory[i].key == key) {
                    return i;
                }
            } 
            return -1;
        }
        int ShareProtectedIntValue(string key) {
            for (int i = 0; i < BurnIntMemory.Length; i++) {
                if (BurnIntMemory[i].key == key) {
                    return BurnIntMemory[i].value;
                }
            } 
            return -1;
        }
        float ShareProtectedFloatValue(string key) {
            for (int i = 0; i < BurnFloatMemory.Length; i++) {
                if (BurnFloatMemory[i].key == key) {
                    return BurnFloatMemory[i].value;
                }
            } 
            return -1;
        }
        public void Erase() {
            PlayerPrefs.DeleteAll();
            Debug.Log($"Attention! Delete protocol is alredy started work,");
            OnDataReset?.Invoke();
            Debug.Log($"currently contacting the subscribers.");
            Debug.Log($"Data must be in place. if some data was completly erased, RIP to theese...");
        }
    }
    [System.Serializable]
    public class ProtectedIntMemory {
        public string key;
        public int value;
    }
    [System.Serializable]
    public class ProtectedFloatMemory {
        public string key;
        public float value;
    }
}
