// To be used in JSON.stringify when a field might be bigint
// https://wagmi.sh/react/faq#bigint-serialization
import { Address } from "@starknet-react/chains";
import { getChecksumAddress } from "starknet";

export const replacer = (_key: string, value: unknown) => {
  if (typeof value === "bigint") return value.toString();
  return value;
};

const addressRegex = /^0x[a-fA-F0-9]{40}$/;

export function isAddress(address: string): address is Address {
  return addressRegex.test(address);
}

export function feltToHex(feltBigInt: bigint) {
  return `0x${feltBigInt.toString(16)}`;
}

// Helper function to convert address types to string format
export function convertAddressToString(address: any): string {
  if (!address) return "";
  if (typeof address === 'string') return address;
  if (typeof address === 'bigint') {
    return getChecksumAddress(feltToHex(address));
  }
  // Handle other potential formats
  return String(address);
}

export function isJsonString(str: string) {
  try {
    JSON.parse(str);
    return true;
  } catch (e) {
    return false;
  }
}

/**
 * Normalizes a Starknet address to ensure consistent formatting for comparison
 * Handles both BigInt and string inputs, ensuring all addresses have the same padding
 * 
 * @param address - The address to normalize (can be BigInt, string, or undefined)
 * @returns Normalized address string with 0x prefix and 64 hex characters, or undefined if input is invalid
 * 
 * @example
 * // BigInt input
 * normalizeAddress(1935454182428824696565329857067150740410806160022250388250988816623935081150n)
 * // Returns: "0x04476da194112a23169afd1626e763d85bb1a6ca2ad3942d315681a10dcecabe"
 * 
 * // String input with different padding
 * normalizeAddress("0x4476da194112a23169afd1626e763d85bb1a6ca2ad3942d315681a10dcecabe")
 * // Returns: "0x04476da194112a23169afd1626e763d85bb1a6ca2ad3942d315681a10dcecabe"
 * 
 * // String input already normalized
 * normalizeAddress("0x04476da194112a23169afd1626e763d85bb1a6ca2ad3942d315681a10dcecabe")
 * // Returns: "0x04476da194112a23169afd1626e763d85bb1a6ca2ad3942d315681a10dcecabe"
 */
export const normalizeAddress = (address: BigInt | string | undefined | null): string | undefined => {
  if (!address) return undefined;

  try {
    let hexString: string;

    if (typeof address === 'bigint') {
      // Convert BigInt to hex string
      hexString = address.toString(16);
    } else if (typeof address === 'string') {
      // Remove 0x prefix if present and clean the string
      hexString = address.toLowerCase().replace(/^0x/, '');

      // Validate that it's a valid hex string
      if (!/^[0-9a-f]*$/.test(hexString)) {
        console.warn(`Invalid hex address format: ${address}`);
        return undefined;
      }
    } else {
      console.warn(`Unsupported address type: ${typeof address}`);
      return undefined;
    }

    // Ensure the hex string is exactly 64 characters (pad with leading zeros)
    const paddedHex = hexString.padStart(64, '0');

    // Return with 0x prefix
    return `0x${paddedHex}`;
  } catch (error) {
    console.error(`Error normalizing address ${address}:`, error);
    return undefined;
  }
};

/**
 * Compares two Starknet addresses for equality, handling different formatting
 * 
 * @param address1 - First address to compare
 * @param address2 - Second address to compare
 * @returns true if addresses are the same, false otherwise
 * 
 * @example
 * // Different padding but same address
 * addressesEqual(
 *   "0x4476da194112a23169afd1626e763d85bb1a6ca2ad3942d315681a10dcecabe",
 *   "0x04476da194112a23169afd1626e763d85bb1a6ca2ad3942d315681a10dcecabe"
 * ); // Returns: true
 * 
 * // BigInt vs string
 * addressesEqual(
 *   1935454182428824696565329857067150740410806160022250388250988816623935081150n,
 *   "0x04476da194112a23169afd1626e763d85bb1a6ca2ad3942d315681a10dcecabe"
 * ); // Returns: true
 */
export const addressesEqual = (
  address1: BigInt | string | undefined | null,
  address2: BigInt | string | undefined | null
): boolean => {
  const normalized1 = normalizeAddress(address1);
  const normalized2 = normalizeAddress(address2);

  if (!normalized1 || !normalized2) return false;

  return normalized1 === normalized2;
};
