﻿using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// DB関連定義クラス
/// </summary>
public static class DbDefine
{
    /// <summary>
    /// 無効な主キーID(有効な主キーは0より大きい整数値とする)
    /// </summary>
    public const int DB_INVALID_PRIMARY_ID = 0;

    /// <summary>
    /// 真値
    /// </summary>
    public const int DB_VALUE_TRUE = 1;

    /// <summary>
    /// 偽値
    /// </summary>
    public const int DB_VALUE_FALSE = 0;
}

/// <summary>
/// DBデータ基底クラス
/// </summary>
public abstract class AbstractData
{
    // 主キー
    public int primaryId = -1;

    /// <summary>
    /// ログ出力
    /// </summary>
    public abstract void DebugPrint();
}

/// <summary>
/// DBテーブル基底クラス
/// </summary>
/// <typeparam name="T">使用するデータクラス</typeparam>
public abstract class AbstractDbTable<T> where T : AbstractData
{
    // データベース
    protected SqliteDatabase db;

    /// <summary>
    /// テーブル名
    /// </summary>
    protected abstract string TableName { get; }

    /// <summary>
    /// 主キー名(大抵は"id"としている、必要ならoverrideすること)
    /// </summary>
    protected virtual string PrimaryKeyName => "id";

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="db">データベース</param>
    public AbstractDbTable(SqliteDatabase db)
    {
        this.db = db;
    }

    /// <summary>
    /// デバッグログ出力
    /// </summary>
    public void DebugPrint()
    {
        foreach (AbstractData data in SelectAll())
        {
            data.DebugPrint();
        }
    }

    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool Save(T data)
    {
        // 有効な主キー(0より大きい整数)でなければ何もしない
        if (data.primaryId <= DbDefine.DB_INVALID_PRIMARY_ID)
        {
            UnityEngine.Debug.LogErrorFormat("無効な主キー: {0}", data.primaryId);
            return false;
        }

        // 特定のレコードが既にあるかどうかチェックする
        AbstractData selectData = SelectFromPrimaryKey(data.primaryId);
        if (selectData != null)
        {
            // データの更新
            return Update(data);
        }
        else
        {
            // データの新規登録
            if (Insert(data))
            {
                // データ追加時のIDをUnitに反映
                int lastId = GetLastId();
                data.primaryId = lastId;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 新規データの保存
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public abstract bool Insert(T data);

    /// <summary>
    /// 既存データの更新
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public abstract bool Update(T data);

    /// <summary>
    /// 古いデータを新しいDBにマージする
    /// </summary>
    /// <param name="oldDb">古い方のデータベース</param>
    public abstract void MargeData(ref SqliteDatabase oldDb);

    /// <summary>
    /// 主キーを指定して該当するデータを取得する
    /// </summary>
    /// <param name="id">主キー</param>
    /// <returns>データ、ただし存在しない場合はnull</returns>
    public T SelectFromPrimaryKey(int id)
    {
        StringBuilder query = new StringBuilder();
        query.Append("SELECT * FROM ");
        query.Append(TableName);
        query.Append(" WHERE ");
        query.Append(PrimaryKeyName);
        query.Append("=");
        query.Append(id.ToString());
        query.Append(";");

        DataTable dt = db.ExecuteQuery(query.ToString());
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        else
        {
            return PutData(dt[0]);
        }
    }

    /// <summary>
    /// データの最後のIDを取得する
    /// </summary>
    /// <returns></returns>
    public int GetLastId()
    {
        DataTable dt = db.ExecuteQuery(string.Format("SELECT id FROM {0} ORDER BY ROWID DESC LIMIT 1", TableName));
        if (dt.Rows.Count == 0)
        {
            return -1;
        }
        else
        {
            return GetIntValue(dt[0], "id");
        }
    }

    /// <summary>
    /// 全データを取得する
    /// </summary>
    /// <returns>データリスト</returns>
    public List<T> SelectAll(string where = null)
    {
        List<T> dataList = new List<T>();
        StringBuilder query = new StringBuilder();
        query.Append("SELECT * FROM ");
        query.Append(TableName);
        if (!string.IsNullOrEmpty(where))
        {
            query.Append(string.Format(" WHERE {0}", where));
        }

        query.Append(";");
        DataTable dt = db.ExecuteQuery(query.ToString());
        foreach (DataRow row in dt.Rows)
        {
            dataList.Add(PutData(row));
        }
        return dataList;
    }

    /// <summary>
    /// 主キーを指定して該当するデータを削除する
    /// </summary>
    /// <param name="id">主キー</param>
    public void DeleteFromPrimaryKey(int id)
    {
        StringBuilder query = new StringBuilder();
        query.Append("DELETE FROM ");
        query.Append(TableName);
        query.Append(" WHERE ");
        query.Append(PrimaryKeyName);
        query.Append("=");
        query.Append(id.ToString());
        query.Append(";");
        db.ExecuteNonQuery(query.ToString());
    }

    /// <summary>
    /// 全データを削除する
    /// </summary>
    public void DeleteAll()
    {
        StringBuilder query = new StringBuilder();
        query.Append("DELETE FROM ");
        query.Append(TableName);
        query.Append(";");
        db.ExecuteNonQuery(query.ToString());
    }

    /// <summary>
    /// 列データをデータクラスに詰め込む
    /// </summary>
    /// <param name="row">列データ</param>
    /// <returns>データクラスのインスタンス</returns>
    protected abstract T PutData(DataRow row);

    /// <summary>
    /// 列データから値を取得する
    /// </summary>
    /// <param name="row">列データ</param>
    /// <param name="key">値キー</param>
    protected int GetIntValue(DataRow row, string key)
    {
        return GetIntValue(row, key, 0);
    }

    /// <summary>
    /// 列データから値を取得する
    /// </summary>
    /// <param name="row">列データ</param>
    /// <param name="key">値キー</param>
    /// <param name="defVal">値が存在しなかった場合のデフォルト値</param>
    protected int GetIntValue(DataRow row, string key, int defVal)
    {
        try
        {
            return (int)row[key];
        }
        catch (NullReferenceException)
        {
            return defVal;
        }
    }

    /// <summary>
    /// 列データから値を取得する
    /// </summary>
    /// <param name="row">列データ</param>
    /// <param name="key">値キー</param>
    protected bool GetBoolValue(DataRow row, string key)
    {
        return GetBoolValue(row, key, false);
    }

    /// <summary>
    /// 列データから値を取得する
    /// </summary>
    /// <param name="row">列データ</param>
    /// <param name="key">値キー</param>
    /// <param name="defVal">値が存在しなかった場合のデフォルト値</param>
    protected bool GetBoolValue(DataRow row, string key, bool defVal)
    {
        try
        {
            return ((int)row[key]) == DbDefine.DB_VALUE_TRUE ? true : false;
        }
        catch (NullReferenceException)
        {
            return defVal;
        }
    }

    /// <summary>
    /// 列データから値を取得する
    /// </summary>
    /// <param name="row">列データ</param>
    /// <param name="key">値キー</param>
    protected string GetStringValue(DataRow row, string key)
    {
        return GetStringValue(row, key, "");
    }

    /// <summary>
    /// 列データから値を取得する
    /// </summary>
    /// <param name="row">列データ</param>
    /// <param name="key">値キー</param>
    /// <param name="defVal">値が存在しなかった場合のデフォルト値</param>
    protected string GetStringValue(DataRow row, string key, string defVal)
    {
        try
        {
            string getVal = (string)row[key];
            if (getVal != null)
            {
                return getVal;
            }
            else
            {
                return defVal;
            }
        }
        catch (NullReferenceException)
        {
            return defVal;
        }
    }
}