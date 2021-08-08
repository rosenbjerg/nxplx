import { h } from 'preact';


interface Props {
	primaryColor: string;
	secondaryColor: string;
}

const Logo = (props: Props) => {
	return (
		<svg version="1.0" xmlns="http://www.w3.org/2000/svg" style="padding: 2px 2px 2px 4px" height="100%" viewBox="0 0 2910 1200"
			 preserveAspectRatio="xMidYMid meet">
			<defs>
				<linearGradient id="gradient" x1="0%" y1="0%" x2="35%" y2="75%" gradientUnits="userSpaceOnUse">
					<stop offset="0%" stopColor={props.secondaryColor} stopOpacity={1} />
					<stop offset="100%" stopColor={props.primaryColor} stopOpacity={1} />
				</linearGradient>
			</defs>
			<g id="layer101" fill="url(#gradient)" stroke="none">
				<path
					d="M275 1171 c-101 -34 -187 -110 -228 -200 -25 -57 -18 -139 18 -214 47 -98 78 -118 49 -33 -47 137 40 308 182 359 46 16 123 17 1090 17 l1041 0 20 -22 c24 -27 182 -246 186 -257 1 -4 -31 -47 -70 -95 -73 -87 -84 -111 -61 -134 29 -29 56 -13 129 74 40 47 75 81 79 77 4 -4 33 -43 65 -86 53 -71 61 -78 89 -75 26 3 31 8 34 33 2 25 -10 48 -64 118 l-67 87 59 70 c81 97 92 116 78 137 -6 10 -22 19 -36 21 -22 3 -38 -11 -94 -77 -38 -45 -71 -81 -74 -81 -3 1 -52 67 -111 148 l-105 147 -1080 2 c-998 2 -1083 0 -1129 -16z" />
				<path
					d="M582 1038 c-9 -9 -12 -94 -12 -319 0 -266 2 -310 16 -323 19 -20 48 -20 67 -2 8 8 93 119 188 245 l174 231 3 -232 c1 -150 6 -236 13 -245 17 -20 56 -15 73 9 14 19 16 67 16 318 0 325 -2 334 -55 328 -26 -3 -54 -35 -210 -242 l-180 -239 -3 228 c-1 148 -6 233 -13 242 -14 16 -61 17 -77 1z" />
				<path
					d="M1236 1034 c-12 -30 -5 -43 64 -131 67 -86 69 -88 51 -108 -74 -83 -131 -158 -131 -173 0 -25 19 -42 46 -42 16 0 44 25 95 85 39 47 72 85 74 85 1 0 30 -38 65 -86 58 -78 65 -85 94 -82 26 3 31 7 34 33 2 25 -10 48 -64 118 l-67 87 59 70 c81 97 92 116 78 137 -6 10 -22 19 -36 21 -22 3 -38 -11 -94 -77 -38 -45 -71 -81 -74 -81 -3 0 -33 36 -68 80 -52 66 -68 80 -92 80 -16 0 -30 -7 -34 -16z" />
				<path
					d="M1750 1024 c-6 -14 -10 -150 -10 -312 0 -249 2 -291 16 -311 16 -22 20 -23 149 -19 126 3 134 4 180 33 64 40 97 101 98 182 0 75 -16 122 -56 161 -49 48 -94 62 -197 62 l-90 0 0 103 c0 112 -6 127 -52 127 -21 0 -31 -6 -38 -26z m301 -330 c39 -39 48 -96 24 -147 -25 -54 -56 -69 -151 -75 l-84 -5 0 133 0 132 90 -4 c85 -3 91 -5 121 -34z" />
				<path
					d="M2306 1029 c-54 -42 -56 -55 -56 -352 0 -287 2 -297 45 -297 43 0 45 11 45 274 0 282 3 298 60 306 29 4 36 9 38 33 2 16 -2 35 -9 43 -18 22 -91 18 -123 -7z" />
			</g>
			<g id="layer102" fill="url(#gradient)" stroke="none">
				<path
					d="M15 753 c22 -158 147 -289 306 -322 l56 -12 17 -54 c37 -117 145 -239 264 -296 171 -82 361 -72 519 27 28 18 69 49 91 68 l38 35 51 -24 c59 -29 162 -55 215 -55 41 0 71 29 60 59 -8 24 -23 31 -63 31 -52 0 -135 27 -206 66 -34 19 -67 34 -73 34 -7 0 -40 -29 -73 -63 -107 -110 -257 -164 -389 -139 -169 32 -307 149 -353 302 -31 101 -26 97 -99 102 -135 11 -274 110 -327 231 -13 31 -28 57 -33 57 -4 0 -5 -21 -1 -47z" />
				<path
					d="M1108 269 c-40 -30 -135 -59 -193 -59 -52 0 -150 24 -172 42 -21 17 -58 5 -61 -20 -3 -18 5 -26 42 -42 137 -61 282 -44 404 47 32 24 43 53 20 53 -7 0 -25 -10 -40 -21z" />
			</g>
		</svg>
	);
};
export default Logo;