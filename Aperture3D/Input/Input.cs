using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Input;
using System.Collections.Generic;

namespace Aperture3D
{
	public static class Input
	{
#if FRONT_TOUCH
		public static List<TouchData> FrontTouchData;
#endif
#if REAR_TOUCH
		public static List<TouchData> RearTouchData;	
#endif
		
		public static float AnalogRightY, AnalogRightX, AnalogLeftY, AnalogLeftX;
		
		public static List<TouchData> TouchData{
		get{
#if !REAR_TOUCH && FRONT_TOUCH
				return FrontTouchData;
#elif REAR_TOUCH && !FRONT_TOUCH
				return RearTouchData;
#else
				return FrontTouchData;
#endif
			}
		}
		
		private static GamePadData gamepadData;
		
		public static MotionData motionData;
		
		public static void UpdateMotionData()
		{
			try{
				motionData = Motion.GetData(0);
			}catch(Exception){}
		}
		
		public static void UpdateTouchData()
		{
#if FRONT_TOUCH
			try{
			FrontTouchData = Touch.GetData(0);
			}catch(Exception){}
#endif
#if REAR_TOUCH
			try{
			RearTouchData = Touch.GetRearTouchData(0);
			}catch(Exception){}
#endif
		}
		public static void UpdateGamepad()
		{
			try{
			gamepadData = GamePad.GetData(0);
			
			AnalogLeftX = gamepadData.AnalogLeftX;
			AnalogLeftY = gamepadData.AnalogLeftY;
			AnalogRightX = gamepadData.AnalogRightX;
			AnalogRightY = gamepadData.AnalogRightY;
				
#if ANALOG_RIGHT_EMU
				if(ButtonsAreDown(GamePadButtons.Left))AnalogRightX = -1;
				else if (ButtonsAreDown(GamePadButtons.Right))AnalogRightX = 1;
				
				if(ButtonsAreDown(GamePadButtons.Up))AnalogRightY = -1;
				else if (ButtonsAreDown(GamePadButtons.Down))AnalogRightY = 1;
#elif ANALOG_LEFT_EMU
				if(ButtonsAreDown(GamePadButtons.Left))AnalogLeftX = -1;
				else if (ButtonsAreDown(GamePadButtons.Right))AnalogLeftX = 1;
				
				if(ButtonsAreDown(GamePadButtons.Up))AnalogLeftY = -1;
				else if (ButtonsAreDown(GamePadButtons.Down))AnalogLeftY = 1;
#endif
				
			}catch(Exception){}
			
		}
		
		public static bool ButtonsAreDown(GamePadButtons buttons)
		{
			return (gamepadData.Buttons.HasFlag(buttons));
		}
	}
}

