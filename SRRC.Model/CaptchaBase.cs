namespace SRRC.Model
{
    /// <summary>
    ///
    /// </summary>
    public abstract class CaptchaBase
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string CaptchaText { set; get; }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string CaptchaToken { set; get; }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string CaptchaInputText { set; get; }
    }
}