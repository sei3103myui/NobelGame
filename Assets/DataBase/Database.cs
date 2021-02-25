using UnityEngine;

/// <summary>
/// Sqliteデータベース管理クラス
/// </summary>
public class Database
{
    private const int DB_VERSION = 1; // DBバージョン 
    private const string DB_NAME = "playerdata.db"; // DBファイル名    
    private const string BACKUP_DB_NAME = "playerdata_backup.db"; // バックアップDBファイル名

    public bool IsInit { get; private set; }

    private static Database instance;
    public static Database Instance
    {
        get
        {
            if (instance == null) instance = new Database();
            return instance;
        }
    }

    public PlayerUnitTable PlayerUnitTable { get; private set; }

    private SqliteDatabase db; // DBアクセスクラス

    /// <summary>
    /// 外部参照不可のコンストラクタ
    /// </summary>
    private Database() { }

    /// <summary>
    /// データベース初期化
    /// </summary>
    /// <param name="mono">呼び出し元インスタンス</param>
    public void Init()
    {
        Debug.Log("DBを確認中です");

        bool isDbUpdate = false; // DB更新フラグ
        bool isDbVersionUpdate = false; // DBバージョン更新フラグ
        int oldDbVersion = UserData.DatabaseVersion; // 端末内のDBバージョンを取得

        // DB保存先パス
        string dbPath = System.IO.Path.Combine(Application.persistentDataPath, DB_NAME);

        // DB更新確認
        CheckDbUpdate(dbPath, oldDbVersion, DB_VERSION, ref isDbUpdate, ref isDbVersionUpdate);

        if (isDbUpdate)
        {
            Debug.Log("DBをアップデートします");
            Debug.Log("バックアップを作成します");

            // コピーを作成する
            string backupPath = System.IO.Path.Combine(Application.persistentDataPath, BACKUP_DB_NAME);
            System.IO.File.Copy(dbPath, backupPath, true);
        }

        // DBを読み込む
        db = new SqliteDatabase(DB_NAME, isDbUpdate || isDbVersionUpdate);

        // テーブルクラスの準備        
        InitTables();

        // 古いDBから新しいDBにデータを移行する
        if (isDbUpdate)
        {
            Debug.Log("テーブルのマージ処理を開始します");
            SqliteDatabase backupDb = new SqliteDatabase(BACKUP_DB_NAME, false);
            MargeData(ref backupDb);
        }

        // DBバージョンを更新する
        if (isDbVersionUpdate)
        {
            Debug.Log("DBのバージョンを変更します " + oldDbVersion + " > " + DB_VERSION);
            UserData.DatabaseVersion = DB_VERSION;
            UserData.Save();
        }

        IsInit = true;
        Debug.Log("DBの起動が完了しました");
    }

    /// <summary>
    /// DB更新確認
    /// </summary>
    /// <param name="dbPath">DBファイルパス</param>
    /// <param name="oldDbVersion">現在のバージョン</param>
    /// <param name="newDbVersion">新バージョン</param>
    /// <param name="isDbUpdate">DB更新フラグ</param>
    /// <param name="isDbVersionUpdate">DBバージョン更新フラグ</param>
    private static void CheckDbUpdate(string dbPath, int oldDbVersion, int newDbVersion, ref bool isDbUpdate, ref bool isDbVersionUpdate)
    {
        Debug.Log("DBの更新を確認します。");
        if (System.IO.File.Exists(dbPath))
        {
            Debug.Log(string.Concat(dbPath, " をチェック中"));
            // DBファイルが存在する場合(2回目以降の起動)
            // DBバージョンが更新されている場合と、DBファイルのタイムスタンプが更新されている場合、2つの可能性をチェックする

            if (oldDbVersion != newDbVersion)
            {
                Debug.Log(string.Concat("DBバージョンが", oldDbVersion, " から ", newDbVersion, " に変更されました"));
                // 定義してあるバージョンとユーザーデータに保持しているバージョンが異なれば更新フラグを立てる
                isDbUpdate = true;
                isDbVersionUpdate = true;
            }

            // DBファイルのタイムスタンプを確認する
            string srcDbPath = System.IO.Path.Combine(Application.streamingAssetsPath, DB_NAME);
            if (System.IO.File.GetLastWriteTimeUtc(srcDbPath) > System.IO.File.GetLastWriteTimeUtc(dbPath))
            {
                Debug.Log(string.Concat(DB_NAME, " のタイムスタンプが更新されました"));
                isDbUpdate = true;
            }
        }
        else
        {
            Debug.Log(string.Concat(dbPath, " が存在しません"));
            // DBファイルが存在しない場合(初回起動)
            isDbVersionUpdate = true;
        }
    }

    /// <summary>
    /// 全テーブルクラスの初期化
    /// </summary>
    private void InitTables()
    {
        PlayerUnitTable = new PlayerUnitTable(db);
    }

    /// <summary>
    /// テーブルごとのマージ（旧=>新）
    /// </summary>
    /// <param name="oldDb"></param>
    private void MargeData(ref SqliteDatabase oldDb)
    {
        PlayerUnitTable.MargeData(ref oldDb);
    }
}
