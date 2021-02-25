using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// ユニットステータス(保存データ)
/// </summary>
[System.Serializable]
public class Unit : AbstractData
{
    public string name; // ユニットの名前
    public int level; // レベル
    public bool isAlive; // 生存
   

    public override void DebugPrint()
    {
    }
}

/// <summary>
/// PlayerUnitテーブルの管理
/// </summary>
public class PlayerUnitTable : AbstractDbTable<Unit>
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="db"></param>
    public PlayerUnitTable(SqliteDatabase db) : base(db) { }

    /// <summary>
    /// テーブル名
    /// </summary>
    protected override string TableName => "player_unit";

    // カラム
    public const string COL_ID = "id";
    public const string COL_UNIT_NAME = "name";
    //public const string COL_ARMY_TYPE = "army_type";
    //public const string COL_CLASS_TYPE = "class_type";
    //public const string COL_AI_TYPE = "ai_type";
    //public const string COL_CONDITION = "condition";
    public const string COL_ALIVE = "alive";

    public const string COL_LEVEL = "level";
    //public const string COL_EXP = "exp";
    //public const string COL_VITALITY = "vitality";
    //public const string COL_ATTACK = "attack";
    //public const string COL_TECHNIQUE = "technique";
    //public const string COL_AGILITY = "agility";
    //public const string COL_DEFENSE = "defense";
    //public const string COL_RESIST = "regist";
    //public const string COL_LUCK = "luck";

    //public const string COL_FACE_IMAGE_NAME = "face_image_name";
    //public const string COL_UNIT_IMAGE_NAME = "unit_image_name";

    /// <summary>
    /// 新規データ登録
    /// </summary>
    /// <param name="data"></param>
    public override bool Insert(Unit data)
    {
        // クエリの作成
        StringBuilder query = new StringBuilder();
        query.Append("INSERT INTO ");
        query.Append(TableName);
        query.Append("(");

        query.Append(COL_UNIT_NAME);
        query.Append(",");
        query.Append(COL_LEVEL);
        query.Append(",");
        query.Append(COL_ALIVE);

        query.Append(")VALUES(");

        query.Append(string.Format("'{0}'", data.name));
        query.Append(",");
        query.Append(data.level);
        query.Append(",");
        query.Append(data.isAlive ? DbDefine.DB_VALUE_TRUE : DbDefine.DB_VALUE_FALSE);
        query.Append(");");
        Debug.Log(query.ToString());
      
        bool isInsert = false;
        db.TransactionStart(); // トランザクション開始
        try
        {
            db.ExecuteNonQuery(query.ToString()); // クエリ実行    
            isInsert = true;
        }
        catch (SqliteException ex)
        {
            db.TransactionRollBack(); // 例外時、ロールバック
            Debug.LogError(ex);
            Debug.LogError(query.ToString());
        }
        db.TransactionCommit(); // コミット

        return isInsert;
    }

    /// <summary>
    /// 既存データ更新
    /// </summary>
    /// <param name="data"></param>
    public override bool Update(Unit data)
    {
        // クエリの作成
        StringBuilder query = new StringBuilder();
        query.Append("UPDATE ");
        query.Append(TableName);
        query.Append(" SET ");

        query.Append(COL_UNIT_NAME);
        query.Append("=");
        query.Append(string.Format("'{0}'", data.name));
        query.Append(",");

        query.Append(COL_LEVEL);
        query.Append("=");
        query.Append(data.level);
        query.Append(",");

        query.Append(COL_ALIVE);
        query.Append("=");
        query.Append(data.isAlive ? DbDefine.DB_VALUE_TRUE : DbDefine.DB_VALUE_FALSE);
        

     
        query.Append(" WHERE ");
        query.Append(COL_ID);
        query.Append("=");
        query.Append(data.primaryId);
        query.Append(";");

        bool result = false;
        db.TransactionStart(); // トランザクション開始
        try
        {
            db.ExecuteNonQuery(query.ToString()); // クエリ実行    
            result = true;
        }
        catch (SqliteException ex)
        {
            db.TransactionRollBack(); // 例外時、ロールバック
            Debug.LogError(ex);
        }
        db.TransactionCommit(); // コミット

        return result;
    }

    /// <summary>
    /// 1行分のレコードを取得する
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    protected override Unit PutData(DataRow data)
    {
        Unit status = new Unit
        {
            primaryId = GetIntValue(data, COL_ID),
            name = GetStringValue(data, COL_UNIT_NAME),
            level = GetIntValue(data, COL_LEVEL),
            isAlive = GetBoolValue(data, COL_ALIVE),
            
            //exp = GetIntValue(data, COL_EXP),
            //param = new StatusParam
            //{
            //    vitality = GetIntValue(data, COL_VITALITY),
            //    attack = GetIntValue(data, COL_ATTACK),
            //    technical = GetIntValue(data, COL_TECHNIQUE),
            //    agility = GetIntValue(data, COL_AGILITY),
            //    defense = GetIntValue(data, COL_DEFENSE),
            //    resist = GetIntValue(data, COL_RESIST),
            //    luck = GetIntValue(data, COL_LUCK),
            //},
            //faceImageName = GetStringValue(data, COL_FACE_IMAGE_NAME),
            //unitImageName = GetStringValue(data, COL_UNIT_IMAGE_NAME),
        };
        return status;
    }

    /// <summary>
    /// 新旧テーブルのマージ処理
    /// </summary>
    /// <param name="oldDb"></param>
    public override void MargeData(ref SqliteDatabase oldDb)
    {
        // 新DBのインスタンス取得
        PlayerUnitTable unitsMsterTable = Database.Instance.PlayerUnitTable;

        // 旧DBのインスタンス取得
        PlayerUnitTable oldUnitsMasterTable = new PlayerUnitTable(oldDb);

        // ユニットのセーブデータ全て取得
        List<Unit> dt = oldUnitsMasterTable.SelectAll();

        if (0 < dt.Count)
        {
            Unit status;
            foreach (Unit dr in dt)
            {
                status = new Unit
                {
                    primaryId = dr.primaryId,
                    name = dr.name,                              
                    level = dr.level,
                    isAlive = dr.isAlive,                   
                };
                unitsMsterTable.Insert(status);
            }
        }
    }
}
