using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Diagnostics;

public class ScriptDebug : MonoBehaviour{

    static public void Log( string msg )
    {
        StackFrame sf = new StackFrame( 1 , true );
        UnityEngine.Debug.Log( msg + "\n関数名:" + sf.GetMethod().ToString() + "\nファイル名:" + sf.GetFileName() + "\n行番号:" + sf.GetFileLineNumber() );
    }

    static public void LogErrorEx( string msg )
    {
        StackFrame sf = new StackFrame( 1 , true );
        UnityEngine.Debug.LogError( msg + "\n関数名:" + sf.GetMethod().ToString() + "\nファイル名:" + sf.GetFileName() + "\n行番号:" + sf.GetFileLineNumber() );
    }
}
