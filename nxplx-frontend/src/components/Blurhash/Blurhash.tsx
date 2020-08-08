import { h } from "preact";

import BlurhashCanvas from './BlurhashCanvas';

interface Props {
  hash: string;
  /** CSS height, default: 128 */
  height: number | string | 'auto';
  punch?: number;
  resolutionX?: number;
  resolutionY?: number;
  style?: object;
  /** CSS width, default: 128 */
  width: number | string | 'auto';
}

const canvasStyle = {
  position: 'absolute',
  top: 0,
  bottom: 0,
  left: 0,
  right: 0,
  width: '100%',
  height: '100%',
};

const Blurhash = ({ hash, height, width, punch, resolutionX, resolutionY, style, ...rest }: Props) => {
  if (resolutionX && resolutionX <= 0) throw new Error('resolutionX must be larger than zero');
  if (resolutionY && resolutionY <= 0) throw new Error('resolutionY must be larger than zero');

  return (
    <div
      {...rest}
      style={{ display: 'inline-block', height, width, ...style, position: 'relative' }}
    >
      <BlurhashCanvas
        hash={hash}
        height={resolutionY}
        width={resolutionX}
        punch={punch}
        style={canvasStyle}
      />
    </div>
  );
}
export default Blurhash;
