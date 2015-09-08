using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace TwixelAppUniversal
{
    public class VolumeFlyout
    {
        int muteVolume;
        bool isMuted;
        bool wasJustMuted;
        Slider VolumeSlider { get; set; }
        Button MuteButton { get; set; }
        AppBarButton VolumeButton { get; set; }
        MediaElement StreamPlayer { get; set; }

        public VolumeFlyout(Slider volumeSlider,
            Button muteButton,
            AppBarButton volumeButton,
            MediaElement streamPlayer)
        {
            muteVolume = 100;
            isMuted = false;
            wasJustMuted = false;
            VolumeButton = volumeButton;
            VolumeSlider = volumeSlider;
            MuteButton = muteButton;
            StreamPlayer = streamPlayer;
            volumeSlider.ValueChanged += VolumeSlider_ValueChanged;
            muteButton.Click += MuteButton_Click;
        }

        private void MuteButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!isMuted)
            {
                wasJustMuted = true;
                isMuted = true;
                StreamPlayer.Volume = 0;
                VolumeSlider.Value = 0;
                VolumeButton.Label = VolumeSlider.Value.ToString();
                ((SymbolIcon)MuteButton.Content).Symbol = Symbol.Mute;
            }
            else
            {
                isMuted = false;
                StreamPlayer.Volume = muteVolume / 100;
                VolumeButton.Label = muteVolume.ToString();
                VolumeSlider.Value = muteVolume;
                ((SymbolIcon)MuteButton.Content).Symbol = Symbol.Volume;
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!wasJustMuted)
            {
                if (isMuted)
                {
                    isMuted = false;
                    ((SymbolIcon)MuteButton.Content).Symbol = Symbol.Volume;
                }
                StreamPlayer.Volume = VolumeSlider.Value / 100;
                muteVolume = (int)VolumeSlider.Value;
                VolumeButton.Label = VolumeSlider.Value.ToString();
            }
            wasJustMuted = false;
        }
    }
}
