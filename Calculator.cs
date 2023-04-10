using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Benchmarks;

class BigIntegerCalculator
{

#if DEBUG
    // Mutable for unit testing.
    private static
#else
    private const
#endif
    int CopyToThreshold = 8;

    private static void CopyTail(ReadOnlySpan<uint> left, Span<uint> bits, int start)
    {
        left.Slice(start).CopyTo(bits.Slice(start));
    }

    //////////////////////////////////////////////////////////////////////////
    // Add (trivials)

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void AddEmpty(ReadOnlySpan<uint> left, uint right, Span<uint> bits)
    {
    }

    public static void AddBaseline(ReadOnlySpan<uint> left, uint right, Span<uint> bits)
    {
        Debug.Assert(left.Length >= 1);
        Debug.Assert(bits.Length == left.Length + 1);

        // Executes the addition for one big and one 32-bit integer.
        // Thus, we've similar code than below, but there is no loop for
        // processing the 32-bit integer, since it's a single element.

        long carry = right;

        for (int i = 0; i < left.Length; i++)
        {
            long digit = left[i] + carry;
            bits[i] = unchecked((uint)digit);
            carry = digit >> 32;
        }

        bits[left.Length] = (uint)carry;
    }

    public static void AddNew(ReadOnlySpan<uint> left, uint right, Span<uint> bits)
    {
        Debug.Assert(left.Length >= 1);
        Debug.Assert(bits.Length == left.Length + 1);

        // Switching to managed references helps eliminating
        // index bounds check...
        ref uint resultPtr = ref MemoryMarshal.GetReference(bits);
        ref uint leftPtr = ref MemoryMarshal.GetReference(left);

        // Executes the addition for one big and one 32-bit integer.
        // Thus, we've similar code than below, but there is no loop for
        // processing the 32-bit integer, since it's a single element.

        nint i = 0;
        nint upperBound = left.Length;
        long carry = right;

        if (upperBound <= CopyToThreshold)
        {
            for ( ; i < upperBound; i++)
            {
                carry += Unsafe.Add(ref leftPtr, i);
                Unsafe.Add(ref resultPtr, i) = unchecked((uint)carry);
                carry >>= 32;
            }

            Unsafe.Add(ref resultPtr, upperBound) = unchecked((uint)carry);
        }
        else
        {
            for ( ; i < upperBound; )
            {
                carry += Unsafe.Add(ref leftPtr, i);
                Unsafe.Add(ref resultPtr, i) = unchecked((uint)carry);
                i++;
                carry >>= 32;
                if (carry == 0) break;
            }

            Unsafe.Add(ref resultPtr, upperBound) = unchecked((uint)carry);

            if (i < upperBound)
            {
                CopyTail(left, bits, unchecked((int)i));
            }
        }

    }

    //////////////////////////////////////////////////////////////////////////
    // Add (non trivials)

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void AddEmpty(ReadOnlySpan<uint> left, ReadOnlySpan<uint> right, Span<uint> bits)
    {
    }

    public static void AddBaseline(ReadOnlySpan<uint> left, ReadOnlySpan<uint> right, Span<uint> bits)
    {
        Debug.Assert(left.Length >= right.Length);
        Debug.Assert(bits.Length == left.Length + 1);

        int i = 0;
        long carry = 0L;

        // Switching to managed references helps eliminating
        // index bounds check...
        ref uint leftPtr = ref MemoryMarshal.GetReference(left);
        ref uint resultPtr = ref MemoryMarshal.GetReference(bits);

        // Executes the "grammar-school" algorithm for computing z = a + b.
        // While calculating z_i = a_i + b_i we take care of overflow:
        // Since a_i + b_i + c <= 2(2^32 - 1) + 1 = 2^33 - 1, our carry c
        // has always the value 1 or 0; hence, we're safe here.

        for ( ; i < right.Length; i++)
        {
            long digit = (Unsafe.Add(ref leftPtr, i) + carry) + right[i];
            Unsafe.Add(ref resultPtr, i) = unchecked((uint)digit);
            carry = digit >> 32;
        }
        for ( ; i < left.Length; i++)
        {
            long digit = left[i] + carry;
            Unsafe.Add(ref resultPtr, i) = unchecked((uint)digit);
            carry = digit >> 32;
        }
        Unsafe.Add(ref resultPtr, i) = (uint)carry;
    }

    public static void AddNew(ReadOnlySpan<uint> left, ReadOnlySpan<uint> right, Span<uint> bits)
    {
        Debug.Assert(right.Length >= 1);
        Debug.Assert(left.Length >= right.Length);
        Debug.Assert(bits.Length == left.Length + 1);

        // Switching to managed references helps eliminating
        // index bounds check...
        ref uint resultPtr = ref MemoryMarshal.GetReference(bits);
        ref uint rightPtr = ref MemoryMarshal.GetReference(right);
        ref uint leftPtr = ref MemoryMarshal.GetReference(left);

        nint i = 0;
        nint upperBound = right.Length;
        long carry = 0;

        // Executes the "grammar-school" algorithm for computing z = a + b.
        // While calculating z_i = a_i + b_i we take care of overflow:
        // Since a_i + b_i + c <= 2(2^32 - 1) + 1 = 2^33 - 1, our carry c
        // has always the value 1 or 0; hence, we're safe here.

        do
        {
            carry += Unsafe.Add(ref leftPtr, i);
            carry += Unsafe.Add(ref rightPtr, i);
            Unsafe.Add(ref resultPtr, i) = unchecked((uint)carry);
            carry >>= 32;
            i++;
        } while (i < upperBound);
        upperBound = left.Length;

        if (upperBound <= CopyToThreshold)
        {
            for ( ; i < upperBound; i++)
            {
                carry += Unsafe.Add(ref leftPtr, i);
                Unsafe.Add(ref resultPtr, i) = unchecked((uint)carry);
                carry >>= 32;
            }

            Unsafe.Add(ref resultPtr, upperBound) = unchecked((uint)carry);
        }
        else
        {
            for ( ; i < upperBound; )
            {
                carry += Unsafe.Add(ref leftPtr, i);
                Unsafe.Add(ref resultPtr, i) = unchecked((uint)carry);
                i++;
                carry >>= 32;
                if (carry == 0) break;
            }

            Unsafe.Add(ref resultPtr, upperBound) = unchecked((uint)carry);

            if (i < upperBound)
            {
                CopyTail(left, bits, unchecked((int)i));
            }
        }


    }

    //////////////////////////////////////////////////////////////////////////
    // Sub (trivials)

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void SubtractEmpty(ReadOnlySpan<uint> left, uint right, Span<uint> bits)
    {
    }      


    public static void SubtractBaseline(ReadOnlySpan<uint> left, uint right, Span<uint> bits)
    {
        Debug.Assert(left.Length >= 1);
        Debug.Assert(left[0] >= right || left.Length >= 2);
        Debug.Assert(bits.Length == left.Length);

        // Executes the subtraction for one big and one 32-bit integer.
        // Thus, we've similar code than below, but there is no loop for
        // processing the 32-bit integer, since it's a single element.

        long carry = -right;

        for (int i = 0; i < left.Length; i++)
        {
            long digit = left[i] + carry;
            bits[i] = unchecked((uint)digit);
            carry = digit >> 32;
        }
    }

    public static void SubtractNew(ReadOnlySpan<uint> left, uint right, Span<uint> bits)
    {
        Debug.Assert(left.Length >= 1);
        Debug.Assert(left[0] >= right || left.Length >= 2);
        Debug.Assert(bits.Length == left.Length);

        // Switching to managed references helps eliminating
        // index bounds check...
        ref uint resultPtr = ref MemoryMarshal.GetReference(bits);
        ref uint leftPtr = ref MemoryMarshal.GetReference(left);

        // Executes the addition for one big and one 32-bit integer.
        // Thus, we've similar code than below, but there is no loop for
        // processing the 32-bit integer, since it's a single element.

        nint i = 0;
        nint upperBound = left.Length;
        long carry = -right;

        if (upperBound <= CopyToThreshold)
        {
            for ( ; i < upperBound; i++)
            {
                carry += Unsafe.Add(ref leftPtr, i);
                Unsafe.Add(ref resultPtr, i) = unchecked((uint)carry);
                carry >>= 32;
            }
        }
        else
        {
            for ( ; i < upperBound; )
            {
                carry += Unsafe.Add(ref leftPtr, i);
                Unsafe.Add(ref resultPtr, i) = unchecked((uint)carry);
                i++;
                carry >>= 32;
                if (carry == 0) break;
            }

            if (i < upperBound)
            {
                CopyTail(left, bits, unchecked((int)i));
            }


        }
    }

    //////////////////////////////////////////////////////////////////////////
    // Sub (non trivials)

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void SubtractEmpty(ReadOnlySpan<uint> left, ReadOnlySpan<uint> right, Span<uint> bits)
    {        
    }

    public static void SubtractBaseline(ReadOnlySpan<uint> left, ReadOnlySpan<uint> right, Span<uint> bits)
    {
        Debug.Assert(left.Length >= right.Length);
        //Debug.Assert(Compare(left, right) >= 0);
        Debug.Assert(bits.Length == left.Length);

        int i = 0;
        long carry = 0L;

        // Switching to managed references helps eliminating
        // index bounds check...
        ref uint leftPtr = ref MemoryMarshal.GetReference(left);
        ref uint resultPtr = ref MemoryMarshal.GetReference(bits);

        // Executes the "grammar-school" algorithm for computing z = a - b.
        // While calculating z_i = a_i - b_i we take care of overflow:
        // Since a_i - b_i doesn't need any additional bit, our carry c
        // has always the value -1 or 0; hence, we're safe here.

        for ( ; i < right.Length; i++)
        {
            long digit = (Unsafe.Add(ref leftPtr, i) + carry) - right[i];
            Unsafe.Add(ref resultPtr, i) = unchecked((uint)digit);
            carry = digit >> 32;
        }
        for ( ; i < left.Length; i++)
        {
            long digit = left[i] + carry;
            Unsafe.Add(ref resultPtr, i) = (uint)digit;
            carry = digit >> 32;
        }

        Debug.Assert(carry == 0);
    }

    public static void SubtractNew(ReadOnlySpan<uint> left, ReadOnlySpan<uint> right, Span<uint> bits)
    {
        Debug.Assert(right.Length >= 1);
        Debug.Assert(left.Length >= right.Length);
        //Debug.Assert(Compare(left, right) >= 0);
        Debug.Assert(bits.Length == left.Length);

        // Switching to managed references helps eliminating
        // index bounds check...
        ref uint resultPtr = ref MemoryMarshal.GetReference(bits);
        ref uint rightPtr = ref MemoryMarshal.GetReference(right);
        ref uint leftPtr = ref MemoryMarshal.GetReference(left);

        nint i = 0;
        nint upperBound = right.Length;
        long carry = 0;

        // Executes the "grammar-school" algorithm for computing z = a + b.
        // While calculating z_i = a_i + b_i we take care of overflow:
        // Since a_i + b_i + c <= 2(2^32 - 1) + 1 = 2^33 - 1, our carry c
        // has always the value 1 or 0; hence, we're safe here.

        do
        {
            carry += Unsafe.Add(ref leftPtr, i);
            carry -= Unsafe.Add(ref rightPtr, i);
            Unsafe.Add(ref resultPtr, i) = unchecked((uint)carry);
            carry >>= 32;
            i++;
        } while (i < upperBound);
        upperBound = left.Length;

        if (upperBound <= CopyToThreshold)
        {
            for ( ; i < upperBound; i++)
            {
                carry += Unsafe.Add(ref leftPtr, i);
                Unsafe.Add(ref resultPtr, i) = unchecked((uint)carry);
                carry >>= 32;
            }
        }
        else
        {
            for ( ; carry != 0 && i < upperBound; i++)
            {
                carry += Unsafe.Add(ref leftPtr, i);
                Unsafe.Add(ref resultPtr, i) = unchecked((uint)carry);
                carry >>= 32;
            }

            if (i < upperBound)
            {
                CopyTail(left, bits, unchecked((int)i));
            }
        }


    }

}
