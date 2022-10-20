using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextInputReader : MonoBehaviour {
    public delegate void func(string s);

    string readData = "";
    int maxCharCount = 0;
    bool read = false;
    bool keepInputting = false;
    bool showCursor = false;

    //  for caps lock shit
    [System.Runtime.InteropServices.DllImport("USER32.dll")] public static extern short GetKeyState(int nVirtKey);

    KeyCode prevInput = 0;

    Coroutine keepWaiter = null, keepBuffer = null, cursorShower = null;

    func updateRunner, endRunner;

    private void Update() {
        if(read) {
            //  does not add to the string
            if(Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
                read = false;
                if(updateRunner != null)
                    updateRunner(readData);
                if(endRunner != null)
                    endRunner(readData);
                return;
            }
            if(Input.GetKeyDown(KeyCode.Backspace) || (Input.GetKey(KeyCode.Backspace) && keepInputting && keepBuffer == null && prevInput == KeyCode.Backspace)) {
                var n = readData.ToCharArray();
                readData = "";
                for(int i = 0; i < n.Length - 1; i++)
                    readData += n[i];
                prevInput = KeyCode.Backspace;
            }

            //  adds to the string
            if(Input.GetKeyDown(KeyCode.Space) || (Input.GetKey(KeyCode.Space) && keepInputting && keepBuffer == null && prevInput == KeyCode.Space)) {
                readData += ' ';

                prevInput = KeyCode.Space;
            }

            //  letters
            for(int i = 97; i < 123; i++) {
                if(Input.GetKeyDown((KeyCode)i) || (Input.GetKey((KeyCode)i) && keepInputting && keepBuffer == null && prevInput == (KeyCode)i)) {
                    if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || (GetKeyState(0x14) & 1) > 0)  //  weird shit is caps lock
                        readData += (KeyCode)i;
                    else
                        readData += ((KeyCode)i).ToString().ToLower()[0];


                    prevInput = (KeyCode)i;
                    break;
                }
            }
            //  numbers
            for(int i = 48; i < 58; i++) {
                if(Input.GetKeyDown((KeyCode)i) || (Input.GetKey((KeyCode)i) && keepInputting && keepBuffer == null && prevInput == (KeyCode)i)) {
                    readData += (char)i;
                    prevInput = (KeyCode)i;
                    break;
                }
            }


            //  check if string is over limit
            if(maxCharCount > 0 && readData.Length >= maxCharCount) {
                var arr = readData.ToCharArray();
                readData = "";
                for(int i = 0; i < maxCharCount; i++)
                    readData += arr[i];
            }

            //  adds cursor
            bool hasCursor = showCursor;
            if(cursorShower == null)
                cursorShower = StartCoroutine(cursorAnimation());
            if(showCursor) {
                readData += 'I';
            }

            //  cleanup
            bool hasInput = Input.GetKey(KeyCode.Backspace) || Input.GetKey(KeyCode.Space);
            bool hasContinuedInput = !Input.GetKeyDown(KeyCode.Backspace) && !Input.GetKeyDown(KeyCode.Space);

            for(int i = 97; i < 123; i++) {
                if(Input.GetKey((KeyCode)i))
                    hasInput = true;
                if(Input.GetKeyDown((KeyCode)i))
                    hasContinuedInput = false;
            }
            for(int i = 48; i < 58; i++) {
                if(Input.GetKey((KeyCode)i))
                    hasInput = true;
                if(Input.GetKeyDown((KeyCode)i))
                    hasContinuedInput = false;
            }



            if(updateRunner != null)
                updateRunner(readData);

            //  reset keep when no coninued input
            if(!hasContinuedInput) {
                if(keepWaiter != null)
                    StopCoroutine(keepWaiter);
                if(keepBuffer != null)
                    StopCoroutine(keepBuffer);
                keepBuffer = null;
                keepWaiter = null;
                keepInputting = false;
            }
            //  start keepers when continued input
            else {
                if(keepWaiter == null)
                    keepWaiter = StartCoroutine(waitToKeepInputting());
                if(keepBuffer == null)
                    keepBuffer = StartCoroutine(bufferKeepInput());
            }

            //  no input
            if(!hasInput) {
                if(keepWaiter != null)
                    StopCoroutine(keepWaiter);
                if(keepBuffer != null)
                    StopCoroutine(keepBuffer);
                keepBuffer = null;
                keepWaiter = null;
                keepInputting = false;
            }

            //  removes cursor for next loop
            if(hasCursor) {
                var n = readData.ToCharArray();
                readData = "";
                for(int i = 0; i < n.Length - 1; i++)
                    readData += n[i];
            }
        }
        else if(!read && !string.IsNullOrEmpty(readData))
            readData = "";
    }


    public void startReading(int maxNumOfChars = 0, string startingText = "", func runAfterUpdate = null, func runAtEnd = null) {
        read = true;
        maxCharCount = maxNumOfChars;
        updateRunner = runAfterUpdate;
        endRunner = runAtEnd;
        readData = startingText;
    }

    IEnumerator waitToKeepInputting() {
        yield return new WaitForSeconds(0.25f);

        keepInputting = true;
    }
    IEnumerator bufferKeepInput() {
        yield return new WaitForSeconds(0.01f);
        keepBuffer = null;
    }
    IEnumerator cursorAnimation() {
        yield return new WaitForSeconds(0.25f);

        showCursor = !showCursor;
        cursorShower = null;
    }

    /*
    public IEnumerator reader(func funcRunsAfterEveryUpdate, func funcRunsAtEnd, string prev = "", List<int> prevPressed = null, int cycleTillAuto = 0) {
        if(prevPressed == null)
            prevPressed = new List<int>();

        //  if none of the inputs that end the renaming process are pressed
        if(!(Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2) || Input.GetKey(KeyCode.Return))) {
            var arr = prev.ToCharArray();
            bool hasInput = false;

            //  removes the unpressed characters in prevPressed
            for(int i = prevPressed.Count - 1; i >= 0; i--) {
                if(!Input.GetKey((KeyCode)prevPressed[i])) {
                    prevPressed.RemoveAt(i);
                    i--;
                }
            }
            //  if the user backspaces
            if(Input.GetKey(KeyCode.Backspace)) {
                bool hasPressedBackspace = false;
                foreach(var i in prevPressed) {
                    if(i == (int)KeyCode.Backspace) {
                        hasPressedBackspace = true;
                        break;
                    }
                }
                if(((!hasPressedBackspace && cycleTillAuto == 0) || (hasPressedBackspace && prevPressed.Count == 1 && cycleTillAuto > autoFillThresh)) && !string.IsNullOrEmpty(prev)) {
                    hasInput = false;
                    prev = "";
                    for(int i = 0; i < arr.Length - 1; i++) {
                        prev += arr[i];
                    }

                    if(!hasPressedBackspace)
                        prevPressed.Add((int)KeyCode.Backspace);
                }
            }

            //  if the user adds to the string
            else {
                char next = '?';

                //  if the user enters a letter
                bool hasPressedLetter = false;
                for(int i = 97; i < 123; i++) {
                    foreach(var p in prevPressed) {
                        if(p == i) {
                            hasPressedLetter = true;
                            break;
                        }
                    }
                    if(Input.GetKey((KeyCode)i) && !(next != '?' && hasPressedLetter) && ((!hasPressedLetter && cycleTillAuto == 0) || (hasPressedLetter && cycleTillAuto > autoFillThresh))) {
                        if(!hasPressedLetter) {
                            prevPressed.Add(i);
                        }
                        hasInput = true;
                        next = (char)i;
                    }
                }

                //  if the user enters a number
                bool hasPressedNumber = false;
                for(int i = 48; i < 58; i++) {
                    foreach(var p in prevPressed) {
                        if(p == i) {
                            hasPressedNumber = true;
                            break;
                        }
                    }
                    if(Input.GetKey((KeyCode)i) && !(next != '?' && hasPressedNumber) && ((!hasPressedNumber && cycleTillAuto == 0) || (hasPressedNumber && cycleTillAuto > autoFillThresh))) {
                        if(!hasPressedNumber)
                            prevPressed.Add(i);
                        hasInput = true;
                        next = (char)i;
                    }
                }

                //  if the user spaces
                bool hasPressedSpace = false;
                if(Input.GetKey(KeyCode.Space)) {
                    foreach(var i in prevPressed) {
                        if(i == (int)KeyCode.Space) {
                            hasPressedSpace = true;
                            break;
                        }
                    }
                    if(!(next != '?' && hasPressedSpace) && ((!hasPressedSpace && cycleTillAuto == 0) || (hasPressedSpace && cycleTillAuto > autoFillThresh))) {
                        if(!hasPressedSpace)
                            prevPressed.Add((int)KeyCode.Space);
                        hasInput = true;
                        next = ' ';
                    }
                }

                if(hasInput) {
                    if((next >= 97 && next < 123) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                        next = (char)(next - 32);
                    prev += next;
                }
            }

            if(!hasInput && prevPressed.Count == 0) {
                cycleTillAuto = 0;
            }
            else
                cycleTillAuto++;
            yield return new WaitForEndOfFrame();

            funcRunsAfterEveryUpdate(prev);
            StartCoroutine(reader(funcRunsAfterEveryUpdate, funcRunsAtEnd, prev, prevPressed, cycleTillAuto));
        }
        //  end the loop
        else {
            funcRunsAfterEveryUpdate(prev);
            funcRunsAtEnd(prev);
            yield return prev;
        }
    } */
}
