import { Pressable, StyleSheet, Text, View, useColorScheme } from 'react-native';
import { Colors, Spacing } from '@/constants/theme';
import { useAuth } from '@/contexts/AuthContext';
import { supabase } from '@/lib/supabase';

export default function HomeScreen() {
  const colorScheme = useColorScheme();
  const colors = Colors[colorScheme === 'dark' ? 'dark' : 'light'];
  const styles = createStyles(colors);
  
  const { session } = useAuth();


  async function handleSignOut() {
    await supabase.auth.signOut();
  }

  return (
    <View style={styles.container}>
      <Text style={styles.title}>PropertyOS</Text>

      <Text style={styles.subtitle}>You are signed in.</Text>

      <Text style={styles.email}>{session?.user.email}</Text>

      <Pressable
        style={({ pressed }) => [
          styles.button,
          pressed && styles.buttonPressed,
        ]}
        onPress={handleSignOut}
      >
        <Text style={styles.buttonText}>Sign Out</Text>
      </Pressable>
    </View>
  );
}

const createStyles = (colors: (typeof Colors)[keyof typeof Colors]) =>
  StyleSheet.create({
    container: {
      flex: 1,
      alignItems: 'center',
      justifyContent: 'center',
      padding: Spacing.four,
      backgroundColor: colors.background,
    },
    title: {
      color: colors.text,
      fontSize: 32,
      fontWeight: '700',
      marginBottom: Spacing.one,
    },
    subtitle: {
      color: colors.text,
      fontSize: 18,
      marginBottom: Spacing.one,
    },
    email: {
      color: colors.textSecondary,
      fontSize: 16,
      marginBottom: Spacing.five,
    },
    button: {
      alignItems: 'center',
      backgroundColor: colors.primary,
      borderRadius: 8,
      paddingHorizontal: Spacing.four,
      paddingVertical: 14,
    },
    buttonPressed: {
      opacity: 0.7,
    },
    buttonText: {
      color: colors.primaryText,
      fontSize: 16,
      fontWeight: '600',
    },
  });