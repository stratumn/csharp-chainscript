namespace Stratumn.Chainscript
{
    /// <summary>
    /// ILinkBuilder makes it easy to create links that adhere to the ChainScript spec.
    /// It provides valid default values for required fields and allows the user to set fields to valid values.
    /// </summary>
    public interface ILinkBuilder<T> where T : ILinkBuilder<T>
    {
        /// <summary>
        /// Set the link action.
        /// The action is what caused the link to be created. </summary>
        /// <param name="action"> friendly name of the action. </param>
        T WithAction(string action);

        /// <summary>
        /// Set the link data (custom object containing business logic details). </summary>
        /// <param name="data"> link details. </param>
        T WithData(object data);

        /// <summary>
        /// Set the maximum number of children a link is allowed to have.
        /// By default this is set to -1 to allow any number of children. </summary>
        /// <param name="d"> degree of the link. </param>
        T WithDegree(int d);

        /// <summary>
        /// Set the link metadata (custom object containing business logic details). </summary>
        /// <param name="data"> link metadata. </param>
        T WithMetadata(object data);

        /// <summary>
        /// Set the link's parent. </summary>
        /// <param name="linkHash"> parent's link hash. </param>
        /// <exception cref="Exception">  </exception>
        T WithParent(byte[] linkHash);

        /// <summary>
        /// Set the link's priority. The priority is used to order links. </summary>
        /// <param name="priority"> a positive float. </param>
        /// <exception cref="Exception">  </exception>
        T WithPriority(double priority);

        /// <summary>
        /// (Optional) Set the link process' state.
        /// The process can be in a specific state depending on the actions taken. </summary>
        /// <param name="state"> process state after the link action. </param>
        T WithProcessState(string state);

        /// <summary>
        /// (Optional) A link can reference other links, even if they are from other
        /// processes. </summary>
        /// <param name="refs"> references to relevant links. </param>
        /// <exception cref="Exception">  </exception>
        T WithRefs(LinkReference[] refs);

        /// <summary>
        /// (Optional) Set the link's process step.
        /// It can be used to help deserialize link data or filter link search results. </summary>
        /// <param name="step"> link process step. </param>
        T WithStep(string step);

        /// <summary>
        /// (Optional) A link can be tagged.
        /// Tags are useful to filter link search results. </summary>
        /// <param name="tags"> link tags. </param>
        T WithTags(string[] tags);

        /// <summary>
        /// build the link. </summary>
        /// <exception cref="ChainscriptException">  </exception>
        Link Build();
    }

}
