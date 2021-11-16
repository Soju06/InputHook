﻿using System.Diagnostics;
using System.Runtime.InteropServices;

namespace InputHook.Models {
    /// <summary>
    /// 키보드 모델
    /// </summary>
    public unsafe class KeyboardModel : MarshalByRefObject {
        internal static InputKeys[] input_keys = (InputKeys[])Enum.GetValues(typeof(InputKeys));

        readonly int fixed_data_length;
        readonly byte* input_data;

        /// <summary>
        /// 키 다운 이벤트
        /// </summary>
        public event KeyboardKeyDown? KeyDown;

        /// <summary>
        /// 키 업 이벤트
        /// </summary>
        public event KeyboardKeyUp? KeyUp;

        /// <summary>
        /// 인풋 데이터
        /// </summary>
        public byte* InputData => input_data;

        /// <summary>
        /// 헤드 포인터
        /// </summary>
        public int HeadPTR => 0;
        /// <summary>
        /// 데이터 포인터
        /// </summary>
        public int DataPTR => fixed_data_length;

        bool disposedValue;

        public KeyboardModel() {
            // 필요한 short 배열 수;
            fixed_data_length = input_keys.Length;
            // 메모리 할당 (헤더 + 데이터)
            input_data = (byte*)Marshal.AllocHGlobal(fixed_data_length * sizeof(byte) * 2);
            // 초기화
            // 헤더
            for (int i = 0; i < fixed_data_length; i++)
                input_data[i] = (byte)input_keys[i];
            // 데이터 클리어
            for (int i = 0; i < fixed_data_length; i++)
                input_data[fixed_data_length + i] = 0;
        }

        /// <summary>
        /// 상태를 설정합니다.
        /// </summary>
        /// <param name="key">키</param>
        public virtual bool SetState(InputKeys key, InputKeyState state) {
            input_data[find_data_index((byte)key)] = (byte)state;

            if (state == 0 || state.HasFlag(InputKeyState.Released))
                return KeyUp?.Invoke(this, key, state) ?? true;
            else return KeyDown?.Invoke(this, key, state) ?? true;
        }

        /// <summary>
        /// 상태를 가져옵니다.
        /// </summary>
        /// <param name="key">키</param>
        public virtual InputKeyState GetState(InputKeys key) {
            return (InputKeyState)input_data[find_data_index((byte)key)];
        }

        protected virtual int find_data_index(byte key) {
            for (int i = 0; i < fixed_data_length; i++)
                if (input_data[i] == key) return fixed_data_length + i;
            return -1;
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // 관리 개체 제거
                    try {

                    } catch {

                    }
                }

                // 비관리 개체 제거
                try {
                    // 인풋 데이터 반환
                    Marshal.FreeHGlobal((IntPtr)input_data);
                } catch {

                }

                disposedValue = true;
            }
        }

        ~KeyboardModel() {
            Dispose(disposing: false);
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public delegate bool KeyboardKeyDown(object sender, InputKeys key, InputKeyState state);
    public delegate bool KeyboardKeyUp  (object sender, InputKeys key, InputKeyState state);
}
