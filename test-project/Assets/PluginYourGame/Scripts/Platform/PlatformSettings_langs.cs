#if UNITY_EDITOR
namespace YG.Insides
{
    public static partial class Langs
    {
#if RU_YG2
        public const string projectSettings = "��������� �������";
        public const string t_nameDefining = "��� ��������� - ������������ ��� Scripting Define Symbols � ��� ������ ������������ ��������� ���������.";
        public const string namePlatform = "��� ���������";
        public const string applySettingsProject = "��������� ��������� �������";
        public const string addProjectSettings = "�������������� ��������� �������";
        public const string t_addPlatform = "�������� � �������� ������ �� ������ ��������������� ����������� ���������. ��������� � ������������.";
#else
        public const string projectSettings = "Progect settings";
        public const string t_nameDefining = "Platform name - used for Scripting Define Symbols and for the class implementing the platform interface.";
        public const string namePlatform = "Name platform";
        public const string applySettingsProject = "Apply settings project";
        public const string addProjectSettings = "Add project settings";
        public const string t_addPlatform = "Create and add a preset with your additional platform settings. For more information, see the documentation.";
#endif
    }
}
#endif