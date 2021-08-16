const palette = {
	darkerGray: '#0C0C0E',
	darkGray: '#151718',
	gray: '#1E1F22',
	lightGray: '#60626C',
	red: '#F24646',
	green: '#2BEBC8',
	purple: '#6746ED',
	purpleBlue: '#4657ed',
	purpleBlueHover: '#4150d7',
	purpleBlueActive: '#3c4ac6',
	white: '#fafafa',
	secondaryGray: '#0D0C0F',
	secondaryBlack: '#000000',
};

interface Theme {
	textColorPrimary: string;
	textColorDisabled: string;
	backgroundColorPrimary: string;
	backgroundColorSecondary: string;
	borderColorPrimary: string;
	boxShadowHeader: string;
	buttonPrimaryBackgroundColor: string;
	buttonPrimaryActiveBackgroundColor: string;
	buttonPrimaryHoverBackgroundColor: string;
	buttonPrimaryDisabledBackgroundColor: string;
	palette: Record<string, string>;
}

export const DarkTheme: Theme = {
	backgroundColorPrimary: palette.darkGray,
	backgroundColorSecondary: palette.gray,
	borderColorPrimary: palette.purple,
	textColorPrimary: palette.white,
	textColorDisabled: palette.lightGray,
	boxShadowHeader: '0 0 5px rgba(0, 0, 0, 0.5)',
	buttonPrimaryBackgroundColor: palette.purpleBlue,
	buttonPrimaryActiveBackgroundColor: palette.purpleBlueHover,
	buttonPrimaryHoverBackgroundColor: palette.purpleBlueActive,
	buttonPrimaryDisabledBackgroundColor: palette.darkGray,
	palette,
};