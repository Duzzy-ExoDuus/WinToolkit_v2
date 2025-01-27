﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace Microsoft.Dism
{
    public static partial class DismApi
    {
        /// <summary>
        ///     Describes the metadata of an image.
        /// </summary>
        /// <remarks>
        ///     <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh824797.aspx" />
        ///     typedef struct _DismImageInfo
        ///     {
        ///     DismImageType ImageType;
        ///     UINT ImageIndex;
        ///     PCWSTR ImageName;
        ///     PCWSTR ImageDescription;
        ///     UINT64 ImageSize;
        ///     UINT Architecture;
        ///     PCWSTR ProductName;
        ///     PCWSTR EditionId;
        ///     PCWSTR InstallationType;
        ///     PCWSTR Hal;
        ///     PCWSTR ProductType;
        ///     PCWSTR ProductSuite;
        ///     UINT MajorVersion;
        ///     UINT MinorVersion;
        ///     UINT Build;
        ///     UINT SpBuild;
        ///     UINT SpLevel;
        ///     DismImageBootable Bootable;
        ///     PCWSTR SystemRoot;
        ///     DismLanguage* Language;
        ///     UINT LanguageCount;
        ///     UINT DefaultLanguage Index;
        ///     VOID* CustomizedInfo;
        ///     } DismImageInfo;
        /// </remarks>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
        internal struct DismImageInfo_
        {
            /// <summary>
            ///     A DismImageType Enumeration value such as DismImageTypeWim.
            /// </summary>
            public DismImageType ImageType;

            /// <summary>
            ///     The index number, starting at 1, of the image.
            /// </summary>
            public uint ImageIndex;

            /// <summary>
            ///     The name of the image.
            /// </summary>
            public string ImageName;

            /// <summary>
            ///     A description of the image.
            /// </summary>
            public string ImageDescription;

            /// <summary>
            ///     The size of the image in bytes.
            /// </summary>
            public ulong ImageSize;

            /// <summary>
            ///     The architecture of the image.
            /// </summary>
            public DismProcessorArchitecture Architecture;

            /// <summary>
            ///     The name of the product, for example, "Microsoft Windows Operating System".
            /// </summary>
            public string ProductName;

            /// <summary>
            ///     The edition of the product, for example, "Ultimate".
            /// </summary>
            public string EditionId;

            /// <summary>
            ///     A string identifying whether the installation is a Client or Server type.
            /// </summary>
            public string InstallationType;

            /// <summary>
            ///     The hardware abstraction layer (HAL) type of the operating system.
            /// </summary>
            public string Hal;

            /// <summary>
            ///     The product type, for example, "WinNT".
            /// </summary>
            public string ProductType;

            /// <summary>
            ///     The product suite, for example, "Terminal Server".
            /// </summary>
            public string ProductSuite;

            /// <summary>
            ///     The major version of the operating system.
            /// </summary>
            public uint MajorVersion;

            /// <summary>
            ///     The minor version of the operating system.
            /// </summary>
            public uint MinorVersion;

            /// <summary>
            ///     The build number, for example, "7600".
            /// </summary>
            public uint Build;

            /// <summary>
            ///     The service pack build.
            /// </summary>
            public uint SpBuild;

            /// <summary>
            ///     The service pack number.
            /// </summary>
            public uint SpLevel;

            /// <summary>
            ///     A DismImageBootable Enumeration value such as DismImageBootableYes.
            /// </summary>
            public DismImageBootable Bootable;

            /// <summary>
            ///     The Windows directory.
            /// </summary>
            public string SystemRoot;

            /// <summary>
            ///     An array of DismLanguage Structure objects representing the languages in the image.
            /// </summary>
            public IntPtr Language;

            /// <summary>
            ///     The number of elements in the language array.
            /// </summary>
            public uint LanguageCount;

            /// <summary>
            ///     The index number of the default language.
            /// </summary>
            public uint DefaultLanguageIndex;

            /// <summary>
            ///     The customized information for the image file. A DismWimCustomizedInfo Structure type for a WIM file. NULL for a
            ///     VHD image.
            /// </summary>
            public IntPtr CustomizedInfo;
        }
    }

    /// <summary>
    ///     Represents the metadata of an image.
    /// </summary>
    public sealed class DismImageInfo
    {
        private readonly DismApi.DismImageInfo_ _imageInfo;
        private readonly List<CultureInfo> _languages = new List<CultureInfo>();
        private readonly DismWimCustomizedInfo _wimCustomizedInfo;

        /// <summary>
        ///     Creates an instance of the DismImageInfo class.
        /// </summary>
        /// <param name="imageInfoPtr">A native pointer to a DismImageInfo_ structure.</param>
        internal DismImageInfo(IntPtr imageInfoPtr)
            : this(imageInfoPtr.ToStructure<DismApi.DismImageInfo_>())
        {
        }

        /// <summary>
        ///     Creates an instance of the DismImageInfo class.
        /// </summary>
        /// <param name="imageInfo">A instance of a DismImageInfo_ structure.</param>
        internal DismImageInfo(DismApi.DismImageInfo_ imageInfo)
        {
            _imageInfo = imageInfo;

            foreach (
                var language in _imageInfo.Language.AsEnumerable<DismApi.DismLanguage>((int) _imageInfo.LanguageCount))
            {
                _languages.Add(CultureInfo.GetCultureInfo(language));
            }

            // Parse the OS version from the various fields
            ProductVersion = new Version((int) imageInfo.MajorVersion, (int) imageInfo.MinorVersion,
                (int) imageInfo.Build, (int) imageInfo.SpBuild);

            // Marshal the WIM customized info
            if (_imageInfo.CustomizedInfo != IntPtr.Zero)
            {
                _wimCustomizedInfo = new DismWimCustomizedInfo(_imageInfo.CustomizedInfo);
            }
        }

        /// <summary>
        ///     Gets the architecture of the image.
        /// </summary>
        public DismProcessorArchitecture Architecture
        {
            get { return _imageInfo.Architecture; }
        }

        /// <summary>
        ///     Gets a value indicating whether an image is a bootable image type.
        /// </summary>
        public DismImageBootable Bootable
        {
            get { return _imageInfo.Bootable; }
        }

        /// <summary>
        ///     The customized information for the image file. A <see cref="DismWimCustomizedInfo" /> for a WIM file. null for a
        ///     VHD image.
        /// </summary>
        public DismWimCustomizedInfo CustomizedInfo
        {
            get { return _wimCustomizedInfo; }
        }

        /// <summary>
        ///     Gets the default language of the image.
        /// </summary>
        public CultureInfo DefaultLanguage
        {
            get { return _languages.Count == 0 ? null : _languages[DefaultLanguageIndex]; }
        }

        /// <summary>
        ///     The index number of the default language.
        /// </summary>
        public int DefaultLanguageIndex
        {
            get { return (int) _imageInfo.DefaultLanguageIndex; }
        }

        /// <summary>
        ///     Gets the name of the edition.
        /// </summary>
        public string EditionId
        {
            get { return _imageInfo.EditionId; }
        }

        /// <summary>
        ///     Gets the hardware abstraction layer (HAL) type of the operating system.
        /// </summary>
        public string Hal
        {
            get { return _imageInfo.Hal; }
        }

        /// <summary>
        ///     Gets the description of the image.
        /// </summary>
        public string ImageDescription
        {
            get { return _imageInfo.ImageDescription; }
        }

        /// <summary>
        ///     Gets the index number, starting at 1, of the image.
        /// </summary>
        public int ImageIndex
        {
            get { return (int) _imageInfo.ImageIndex; }
        }

        /// <summary>
        ///     Gets the name of the image.
        /// </summary>
        public string ImageName
        {
            get { return _imageInfo.ImageName; }
        }

        /// <summary>
        ///     Gets the size of the image in bytes.
        /// </summary>
        public ulong ImageSize
        {
            get { return _imageInfo.ImageSize; }
        }

        /// <summary>
        ///     Gets the type of the image.
        /// </summary>
        public DismImageType ImageType
        {
            get { return _imageInfo.ImageType; }
        }

        /// <summary>
        ///     Gets the type of installation such as "Client" or "Server".
        /// </summary>
        public string InstallationType
        {
            get { return _imageInfo.InstallationType; }
        }

        /// <summary>
        ///     Gets the languages in the image.
        /// </summary>
        public IEnumerable<CultureInfo> Languages
        {
            get { return _languages; }
        }

        /// <summary>
        ///     Gets the name of the product.
        /// </summary>
        public string ProductName
        {
            get { return _imageInfo.ProductName; }
        }

        /// <summary>
        ///     Gets thee product suite, for example, "Terminal Server".
        /// </summary>
        public string ProductSuite
        {
            get { return _imageInfo.ProductSuite; }
        }

        /// <summary>
        ///     Gets the product type, for example, "WinNT".
        /// </summary>
        public string ProductType
        {
            get { return _imageInfo.ProductType; }
        }

        /// <summary>
        ///     Gets the version of the operating system contained in the image.
        /// </summary>
        public Version ProductVersion { get; private set; }

        /// <summary>
        ///     Gets the service pack number.
        /// </summary>
        public int SpLevel
        {
            get { return (int) _imageInfo.SpLevel; }
        }

        /// <summary>
        ///     Gets the path to the Windows directory.
        /// </summary>
        public string SystemRoot
        {
            get { return _imageInfo.SystemRoot; }
        }

        /// <summary>
        ///     Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        ///     <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        ///     true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />;
        ///     otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj != null && Equals(obj as DismImageInfo);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="DismImageInfo" /> is equal to the current <see cref="DismImageInfo" />.
        /// </summary>
        /// <param name="imageInfo">The <see cref="DismImageInfo" /> object to compare with the current object.</param>
        /// <returns>
        ///     true if the specified <see cref="DismImageInfo" /> is equal to the current <see cref="DismImageInfo" />;
        ///     otherwise, false.
        /// </returns>
        public bool Equals(DismImageInfo imageInfo)
        {
            return imageInfo != null
                   && Architecture == imageInfo.Architecture
                   && Bootable == imageInfo.Bootable
                   && Equals(CustomizedInfo, imageInfo.CustomizedInfo)
                   && DefaultLanguageIndex == imageInfo.DefaultLanguageIndex
                   && EditionId == imageInfo.EditionId
                   && Hal == imageInfo.Hal
                   && ImageDescription == imageInfo.ImageDescription
                   && ImageIndex == imageInfo.ImageIndex
                   && ImageSize == imageInfo.ImageSize
                   && ImageType == imageInfo.ImageType
                   && InstallationType == imageInfo.InstallationType
                   && Languages.SequenceEqual(imageInfo.Languages)
                   && ProductName == imageInfo.ProductName
                   && ProductSuite == imageInfo.ProductSuite
                   && ProductType == imageInfo.ProductType
                   && ProductVersion == imageInfo.ProductVersion
                   && SpLevel == imageInfo.SpLevel
                   && SystemRoot == imageInfo.SystemRoot;
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
        public override int GetHashCode()
        {
            return Architecture.GetHashCode()
                   ^ Bootable.GetHashCode()
                   ^ CustomizedInfo.GetHashCode()
                   ^ DefaultLanguageIndex
                   ^ (string.IsNullOrWhiteSpace(EditionId) ? 0 : EditionId.GetHashCode())
                   ^ (string.IsNullOrWhiteSpace(Hal) ? 0 : Hal.GetHashCode())
                   ^ (string.IsNullOrWhiteSpace(ImageDescription) ? 0 : ImageDescription.GetHashCode())
                   ^ ImageIndex.GetHashCode()
                   ^ ImageType.GetHashCode()
                   ^ (string.IsNullOrWhiteSpace(InstallationType) ? 0 : InstallationType.GetHashCode())
                   ^ Languages.GetHashCode()
                   ^ (string.IsNullOrWhiteSpace(ProductName) ? 0 : ProductName.GetHashCode())
                   ^ (string.IsNullOrWhiteSpace(ProductSuite) ? 0 : ProductSuite.GetHashCode())
                   ^ (string.IsNullOrWhiteSpace(ProductType) ? 0 : ProductType.GetHashCode())
                   ^ ProductVersion.GetHashCode()
                   ^ SpLevel.GetHashCode()
                   ^ (string.IsNullOrWhiteSpace(SystemRoot) ? 0 : SystemRoot.GetHashCode());
        }
    }

    /// <summary>
    ///     Represents a collection of <see cref="DismImageInfo" /> objects.
    /// </summary>
    public sealed class DismImageInfoCollection : DismCollection<DismImageInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the DismImageInfoCollection that is empty.
        /// </summary>
        internal DismImageInfoCollection()
            : base(new List<DismImageInfo>())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the DismImageInfoCollection based on the specified list.
        /// </summary>
        internal DismImageInfoCollection(IList<DismImageInfo> list)
            : base(list)
        {
        }
    }
}