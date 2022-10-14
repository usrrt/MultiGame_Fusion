using System;

/// <summary>
/// Fusion�� connetion token�� ���� ������ Ŭ����
/// </summary>
public static class ConnectionTokenUtils
{
    /// <summary>
    /// �������� ��ū�� �����Ѵ�(������ ��ū�� �ο��ϱ�����)
    /// </summary>
    /// <returns></returns>
    public static byte[] NewToken() => Guid.NewGuid().ToByteArray();

    /// <summary>
    /// token�� hash�������� ��ȯ
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static int HashToken(byte[] token) => new Guid(token).GetHashCode();

    /// <summary>
    /// token�� string���·� ��ȯ
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static string TokenToString(byte[] token) => new Guid(token).ToString();

}