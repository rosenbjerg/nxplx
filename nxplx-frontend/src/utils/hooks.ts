import { imagesize, imageUrl } from "./models";

export const useBackgroundGradient = (backgroundImageUrl:string, size: imagesize = 1280) => {
    return `background-image: linear-gradient(rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.6)), url("${imageUrl(backgroundImageUrl, size)}");`;
}