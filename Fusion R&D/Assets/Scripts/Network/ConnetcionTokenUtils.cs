using System;

/// <summary>
/// Fusion의 connetion token에 사용될 유용한 클래스
/// </summary>
public static class ConnectionTokenUtils
{
    /// <summary>
    /// 랜덤으로 토큰을 생성한다(고유한 토큰을 부여하기위함)
    /// </summary>
    /// <returns></returns>
    public static byte[] NewToken() => Guid.NewGuid().ToByteArray();

    /// <summary>
    /// token을 hash포맷으로 변환
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static int HashToken(byte[] token) => new Guid(token).GetHashCode();

    /// <summary>
    /// token을 string형태로 변환
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static string TokenToString(byte[] token) => new Guid(token).ToString();

}