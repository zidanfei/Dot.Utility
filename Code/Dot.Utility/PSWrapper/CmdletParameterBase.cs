//-----------------------------------------------------------------------
// <copyright file="CmdletParameterBase.cs" company="Microsoft">
//     Copyright 2010 (c) Microsoft Corporation.  All rights reserved.
//     Generated on 1-13-2010
// </copyright>
// <summary>Implements the CmdletParameterBase classes.</summary>
//-----------------------------------------------------------------------


namespace Dot.Utility.PSWrapper
{
    #region ParameterBase

    public abstract class ParameterBase
    {

        protected ParameterBase()
        {
            this.isSet = false;
            this.parameterValueType = ParameterValueTypeEnum.NotSet;
            this.isSwitch = false;
        }

        private bool isSwitch;
        public bool IsSwitch
        {
            get
            {
                return this.isSwitch;
            }
            protected set
            {
                this.isSwitch = value;
            }
        }

        private bool isSet;
        public bool IsSet
        {
            get
            {
                return this.isSet;
            }
            set
            {
                this.isSet = value;
            }
        }

        private ParameterValueTypeEnum parameterValueType;
        public ParameterValueTypeEnum ParameterValueType
        {
            get
            {
                return this.parameterValueType;
            }
            protected set
            {
                this.parameterValueType = value;
            }
        }

        public abstract object ValueObject { get; }

    }

    #endregion
}

